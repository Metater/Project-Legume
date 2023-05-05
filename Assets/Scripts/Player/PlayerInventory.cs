using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : PlayerComponent
{
    private const int SlotCount = 3;
    [SerializeField] private Color crosshairHoverColor = new (0.2039216f, 0.5647059f, 0.8117647f, 0.4980392f);
    [SerializeField] private float reachDistance;
    private readonly Item[] slots = new Item[SlotCount];
    private int selectedSlotLocal = 0;
    [SyncVar(hook = nameof(OnSelectedSlotChanged))] public int selectedSlotSynced = 0;
    private Item LocallySelectedItem => slots[selectedSlotLocal];

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
                    // TODO Send Pickup Command
                }

                manager.Get<CrosshairManager>().SetColor(crosshairHoverColor);
            }
        }

        UpdateSelectedSlot();
    }

    private void OnSelectedSlotChanged(int oldSelectedSlot, int newSelectedSlot)
    {
        if (oldSelectedSlot >= 0 && oldSelectedSlot < SlotCount && slots[oldSelectedSlot] != null)
        {
            // TODO slots[oldSelectedSlot].Deselect();
        }

        if (newSelectedSlot >= 0 && newSelectedSlot < SlotCount && slots[newSelectedSlot] != null)
        {
            // TODO slots[newSelectedSlot].Select();
        }
    }
    private void UpdateSelectedSlot()
    {
        int originalSelectedSlotLocal = selectedSlotLocal;

        float mouseScrollDelta = Input.mouseScrollDelta.y;
        if (mouseScrollDelta > 0)
        {
            // Scroll up
            selectedSlotLocal++;
        }
        else if (mouseScrollDelta < 0)
        {
            // Scroll down
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
            CmdChangeSelectedSlot((byte)selectedSlotLocal);
        }
    }
    [Command]
    private void CmdChangeSelectedSlot(byte newSelectedSlot)
    {
        if (newSelectedSlot < 0 || newSelectedSlot > SlotCount)
        {
            return;
        }

        selectedSlotSynced = newSelectedSlot;
    }
}
