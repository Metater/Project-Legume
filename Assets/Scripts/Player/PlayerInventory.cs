using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class PlayerInventory : PlayerComponent
{
    private const int SlotCount = 3;
    [SerializeField] private Color crosshairHoverColor = new (0.2039216f, 0.5647059f, 0.8117647f, 0.4980392f);
    [SerializeField] private float reachDistance;
    [SerializeField] private Transform gripTransform;
    [SerializeField] private float itemRotationSlerpMultiplier;
    private readonly Item[] slots = new Item[SlotCount];
    private int selectedSlot = 0;
    private Item SelectedItem { get { return slots[selectedSlot]; } set { slots[selectedSlot] = value; } }
    public Transform GripTransform => gripTransform;

    public override void PlayerUpdate()
    {
        if (isServer)
        {
            foreach (var item in slots)
            {
                if (item == null)
                {
                    continue;
                }

                item.ServerUpdateVisibility(item == SelectedItem);
            }
        }

        if (!isLocalPlayer || !manager.Get<PhaseManager>().ShouldPlayerMove)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (SelectedItem == null && Physics.Raycast(ray, out var hit, reachDistance))
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

        if (SelectedItem != null)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                CmdDropItem();
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

            Quaternion rotation = Quaternion.Slerp(item.transform.rotation, gripTransform.rotation, Time.deltaTime * itemRotationSlerpMultiplier);
            item.transform.SetPositionAndRotation(gripTransform.position, rotation);
        }
    }

    private void UpdateSelectedSlot()
    {
        int originalSelectedSlot = selectedSlot;

        float mouseScrollDelta = Input.mouseScrollDelta.y;
        // Scroll up
        if (mouseScrollDelta > 0)
        {
            selectedSlot++;
        }
        // Scroll down
        else if (mouseScrollDelta < 0)
        {
            selectedSlot--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlot = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlot = 2;
        }

        if (selectedSlot < 0)
        {
            selectedSlot = SlotCount - 1;
        }
        else if (selectedSlot >= SlotCount)
        {
            selectedSlot = 0;
        }

        if (originalSelectedSlot != selectedSlot)
        {
            CmdChangeSelectedSlot(selectedSlot);
        }
    }

    [Command]
    private void CmdChangeSelectedSlot(int selectedSlot)
    {
        if (selectedSlot < 0 || selectedSlot > SlotCount)
        {
            return;
        }

        this.selectedSlot = selectedSlot;
    }
    [Command]
    private void CmdPickupItem(NetworkIdentity itemNetIdentity)
    {
        if (!itemNetIdentity.TryGetComponent(out Item item) || item.IsHeld || SelectedItem != null)
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
            item.netIdentity.AssignClientAuthority(connectionToClient);
        }

        if (item.ServerPickup(player))
        {
            SelectedItem = item;
            TargetPickupItem(item.netIdentity, selectedSlot);
        }
    }
    [Command]
    private void CmdDropItem()
    {
        Item item = SelectedItem;
        if (item == null)
        {
            return;
        }

        if (item.ServerDrop(player))
        {
            SelectedItem = null;
            TargetDropItem(item.netIdentity, selectedSlot);
        }
    }
    [Command]
    private void CmdLeftMouseButtonDown()
    {
        
    }
    [Command]
    private void CmdRightMouseButtonDown()
    {

    }

    [TargetRpc]
    private void TargetPickupItem(NetworkIdentity itemNetIdentity, int slot)
    {
        if (!itemNetIdentity.TryGetComponent(out Item item))
        {
            return;
        }

        item.transform.SetPositionAndRotation(gripTransform.position, gripTransform.rotation);
        slots[slot] = item;
    }
    [TargetRpc]
    private void TargetDropItem(NetworkIdentity itemNetIdentity, int slot)
    {
        if (!itemNetIdentity.TryGetComponent(out Item item))
        {
            return;
        }

        Vector3 vector = Camera.main.transform.forward;
        Vector3 velocity = player.Get<PlayerMovement>().GetVelocity();
        item.ClientAddDropForce(vector, velocity);
        slots[slot] = null;
    }
}
