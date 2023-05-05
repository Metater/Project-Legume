using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : PlayerComponent
{
    private const int SlotCount = 3;
    [SerializeField] private Color crosshairHoverColor = new (0.2039216f, 0.5647059f, 0.8117647f, 0.4980392f);
    [SerializeField] private float reachDistance;
    [SerializeField] private Transform gripTransform;
    [SerializeField] private float itemRotationSlerpMultiplier;
    private readonly Item[] slots = new Item[SlotCount];
    private int selectedSlotLocal = 0;
    [SyncVar(hook = nameof(OnSelectedSlotChanged))] public int selectedSlotSynced = 0;
    private Item ClientSelectedItem => slots[selectedSlotLocal];
    private Item ServerSelectedItem => slots[selectedSlotSynced];

    public override void PlayerUpdate()
    {
        if (!isLocalPlayer || !manager.Get<PhaseManager>().HasStarted || manager.Get<CursorManager>().IsCursorVisable)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, reachDistance))
        {
            if (hit.transform.TryGetComponent<Item>(out var item) && !item.IsHeld)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdPickupItem(item.netIdentity);
                }

                manager.Get<CrosshairManager>().SetColor(crosshairHoverColor);
            }
        }

        UpdateSelectedSlot();

        if (ClientSelectedItem != null)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Vector3 vector = Camera.main.transform.forward;
                Vector3 velocity = player.Get<PlayerMovement>().GetVelocity();
                CmdDropItem(forward, velocity);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                CmdLeftMouseButtonDown();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CmdRightMouseButtonDown();
            }
        }

        foreach (var item in slots)
        {
            if (item == null || !item.isOwned)
            {
                continue;
            }

            Vector3 position = gripTransform.position;
            Quaternion rotation = Quaternion.Slerp(item.transform.rotation, gripTransform.rotation, Time.deltaTime * itemRotationSlerpMultiplier);
            item.transform.SetPositionAndRotation(position, rotation);
        }
    }

    private void OnSelectedSlotChanged(int oldSelectedSlot, int newSelectedSlot)
    {
        if (oldSelectedSlot >= 0 && oldSelectedSlot < SlotCount && slots[oldSelectedSlot] != null)
        {
            slots[oldSelectedSlot].ClientDeselect();
        }

        if (newSelectedSlot >= 0 && newSelectedSlot < SlotCount && slots[newSelectedSlot] != null)
        {
            slots[newSelectedSlot].ClientSelect();
        }
    }
    private void UpdateSelectedSlot()
    {
        int originalSelectedSlotLocal = selectedSlotLocal;

        float mouseScrollDelta = Input.mouseScrollDelta.y;
        // Scroll up
        if (mouseScrollDelta > 0)
        {
            selectedSlotLocal++;
        }
        // Scroll down
        else if (mouseScrollDelta < 0)
        {
            selectedSlotLocal--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotLocal = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotLocal = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotLocal = 2;
        }

        if (selectedSlotLocal < 0)
        {
            selectedSlotLocal = SlotCount - 1;
        }
        else if (selectedSlotLocal >= SlotCount)
        {
            selectedSlotLocal = 0;
        }

        if (originalSelectedSlotLocal != selectedSlotLocal)
        {
            CmdChangeSelectedSlot(selectedSlotLocal);
        }
    }

    [Command]
    private void CmdChangeSelectedSlot(int selectedSlot)
    {
        if (selectedSlot < 0 || selectedSlot > SlotCount)
        {
            return;
        }

        selectedSlotSynced = selectedSlot;
    }
    [Command]
    private void CmdPickupItem(NetworkIdentity item)
    {
        if (item == null || item.IsHeld || ServerSelectedItem != null)
        {
            return;
        }
        
        // Item has owner already
        if (item.netIdentity.connectionToClient != null)
        {
            // Item's owner is incorrect
            if (item.netIdentity.connectionToClient != connectionToClient)
            {
                item.netIdentity.RemoveClientAuthority();
                item.netIdentity.AssignClientAuthority(connectionToClient);
            }
        }
        // Item does not have owner already
        else
        {
            // TODO Disable server rigidbody

            item.netIdentity.AssignClientAuthority(connectionToClient);
        }

        // TODO item.ServerPickup();
    }
    [Command]
    private void CmdDropItem(Vector3 vector, Vector3 velocity)
    {

    }
    [Command]
    private void CmdLeftMouseButtonDown()
    {

    }
    [Command]
    private void CmdRightMouseButtonDown()
    {

    }
}
