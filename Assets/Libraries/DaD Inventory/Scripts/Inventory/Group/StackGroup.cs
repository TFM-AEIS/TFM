using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unite stack cells in group and change it's stack logic.
/// </summary>
public class StackGroup : MonoBehaviour
{
    [Tooltip("Split items when drop between two different stack groups")]
    public bool splitOuter;
    [Tooltip("Arrange item placement in group's cells on drop")]
    public bool arrangeMode;
    [Tooltip("This stack group will destroy any dropped item")]
    public bool trashBinMode;
    [Tooltip("SFX for item destroying")]
    public AudioClip trashBinSound;
    [Tooltip("Audio source for SFX")]
    public AudioSource audioSource;
    [Tooltip("This game objects will be notified on stack events")]
    public List<GameObject> eventAdditionalReceivers = new List<GameObject>();

    public static bool globalSplit;                                             // Split interface will be used for all item's drag till this value == true

    public class StackGroupEventDescriptor                                      // Info about stack group event
    {
        public StackGroup sourceGroup = null;                                   // From this stack group item was dragged
        public StackGroup destinationGroup = null;                              // Into this stack group item was dropped
        public StackCell sourceCell = null;                                     // From this cell item was dragged
        public List<StackCell> destinationCells = new List<StackCell>();        // Between this cells item was destributed
    }

    private class DistributeResults                                             // Item distribution inside group results
    {
        public int amount = 0;                                                  // Distributed items amount
        public List<StackCell> cells = new List<StackCell>();                   // Cells in which items were distributed
    }

    private enum MyState                                                        // Internal state
    {
        WaitForRequest,
        WaitForEvent,
        Busy
    }

    private bool inited = false;
    private MyState myState = MyState.WaitForRequest;                           // State machine
    private static SplitInterface splitInterface;                               // Interface for items split operations
    private static CoroutineContainer coroutineContainer;                       // Container for coroutines (operate even GO is inactive)
    private Coroutine eventHandkerCoroutine = null;                             // Coroutine for drag and drop event operation

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (inited == false)
        {
            inited = true;

            if (splitInterface == null)
            {
                splitInterface = FindObjectOfType<SplitInterface>();
            }
            if (splitInterface == null)
            {
                // Create split interface from Resource folder
                splitInterface = Instantiate(Resources.Load<SplitInterface>("SplitInterface"));
                splitInterface.gameObject.name = "SplitInterface";
                splitInterface.transform.SetAsLastSibling();
                DontDestroyOnLoad(splitInterface.gameObject);
            }

            // Create contaner for coroutines
            if (coroutineContainer == null)
            {
                coroutineContainer = FindObjectOfType<CoroutineContainer>();
                if (coroutineContainer == null)
                {
                    coroutineContainer = new GameObject().AddComponent<CoroutineContainer>();
                    coroutineContainer.name = "CoroutineContainer";
                    DontDestroyOnLoad(coroutineContainer.gameObject);
                }
            }

            Debug.Assert(splitInterface, "Wrong initial settings");
        }
    }

    /// <summary>
    /// Toggles the global split enabling.
    /// </summary>
    public void ToggleGlobalSplit()
    {
        globalSplit = !globalSplit;
    }

    /// <summary>
    /// Create item from prefab and distribute it insede group.
    /// </summary>
    /// <returns>Distributed items amount.</returns>
    /// <param name="stackItemPrefab">Stack item prefab.</param>
    /// <param name="stackAmount">Stack amount for created item.</param>
    public int AddItemFromPrefab(StackItem stackItemPrefab, int stackAmount)
    {
        int res = 0;
        Debug.Log(stackItemPrefab);
        if (stackItemPrefab != null)
        {
            StackGroupEventDescriptor desc = new StackGroupEventDescriptor();
            // Create item
            StackItem stackItem = Instantiate(stackItemPrefab);
            stackItem.name = stackItemPrefab.name;
            // Set stack amount
            stackAmount = Mathf.Min(stackAmount, stackItem.maxStack);
            stackItem.SetStack(stackAmount);
            // Init inernal state
            stackItem.Init();
            // Try to distribute item inside group's items and cells
            DistributeResults distributeResults = DistributeAnywhere(stackItem, stackAmount, null);
            Debug.Log(distributeResults);
            res += distributeResults.amount;
            if (stackItem.GetStackCell() == null)
            {
                // Destroy item if it has no cell (was not distributed completed)
                Destroy(stackItem.gameObject);
            }
            // Send stack event notification
            if (res > 0)
            {
                PlaySound(stackItem.sound);

                desc.sourceGroup = this;
                desc.destinationGroup = this;
                desc.sourceCell = distributeResults.cells[0];
                desc.destinationCells = distributeResults.cells;
                SendNotification(desc);
            }
        }
        return res;
    }

    /// <summary>
    /// Adds existing item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="stackItem">Stack item.</param>
    /// <param name="limit">Limit.</param>
    public int AddExistingItem(StackItem stackItem, int limit)
    {
        int res = 0;
        if (stackItem != null)
        {
            // Init internal state (just in case it was not initted before)
            stackItem.Init();

            StackGroup sourceStackGroup = AccessUtility.GetComponentInParent<StackGroup>(stackItem.transform);
            StackCell sourceStackCell = stackItem.GetStackCell();

            StackGroupEventDescriptor desc = new StackGroupEventDescriptor();
            // Try to distribute item inside group's items and cells
            DistributeResults distributeResults = DistributeAnywhere(stackItem, limit, null);
            res += distributeResults.amount;
            // Send stack event notification
            if (res > 0)
            {
                PlaySound(stackItem.sound);

                desc.sourceGroup = sourceStackGroup;
                desc.destinationGroup = this;
                desc.sourceCell = sourceStackCell;
                desc.destinationCells = distributeResults.cells;
                SendNotification(desc);
            }
        }
        return res;
    }

    /// <summary>
    /// Calls the drag and drop event manually from script.
    /// </summary>
    /// <param name="sourceCell">Source cell.</param>
    /// <param name="destinationCell">Destination cell.</param>
    public void CallDadEventManually(StackCell sourceCell, StackCell destinationCell)
    {
        if (myState == MyState.WaitForRequest)
        {
            if (sourceCell != null && destinationCell != null)
            {
                DadCell.DadEventDescriptor desc = new DadCell.DadEventDescriptor();
                desc.sourceCell = sourceCell.GetComponent<DadCell>();
                desc.destinationCell = destinationCell.GetComponent<DadCell>();
                myState = MyState.Busy;
                // Operate item's drop
                eventHandkerCoroutine = coroutineContainer.StartCoroutine(EventHandler(desc));
            }
        }
    }

    /// <summary>
    /// Calls the drag and drop event manually from script.
    /// </summary>
    /// <param name="sourceCell">Source cell.</param>
    /// <param name="destinationGroup">Destination group.</param>
    public void CallDadEventManually(StackCell sourceCell, StackGroup destinationGroup)
    {
        Init();

        if (myState == MyState.WaitForRequest)
        {
            if (sourceCell != null && destinationGroup != null)
            {
                StackItem stackItem = sourceCell.GetStackItem();
                StackCell destinationCell = null;
                if (stackItem != null)
                {
                    List<StackCell> destinationCells = new List<StackCell>();

                    // Check for not full cells with same items
                    foreach (StackItem similarStackItem in GetSimilarStackItems(stackItem))
                    {
                        if (similarStackItem.name == stackItem.name)
                        {
                            StackCell similarStackCell = similarStackItem.GetStackCell();
                            if (similarStackCell.GetAllowedSpace() > 0)
                            {
                                destinationCell = similarStackCell;
                                break;
                            }
                        }
                    }

                    if (destinationCell == null)
                    {
                        // Check for empty cells
                        destinationCells = GetFreeStackCells(stackItem);
                        if (destinationCells.Count > 0)
                        {
                            destinationCell = destinationCells[0];
                        }
                    }

                    if (destinationCell == null)
                    {
                        // Check for other cells with similar items
                        foreach (StackItem similarStackItem in GetSimilarStackItems(stackItem))
                        {
                            if (similarStackItem.name != stackItem.name)
                            {
                                destinationCell = similarStackItem.GetStackCell();
                                break;
                            }
                        }
                    }

                    if (destinationCell != null)
                    {
                        CallDadEventManually(sourceCell, destinationCell);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes the item from cell.
    /// </summary>
    /// <param name="stackCell">Stack cell.</param>
    /// <param name="limit">Limit.</param>
    public void RemoveItem(StackCell stackCell, int limit)
    {
        if (stackCell != null)
        {
            StackItem stackItem = stackCell.GetStackItem();
            if (stackItem != null)
            {
                RemoveItem(stackItem, limit);
            }
        }
    }

    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="stackItem">Stack item.</param>
    /// <param name="limit">Limit.</param>
    public void RemoveItem(StackItem stackItem, int limit)
    {
        if (stackItem != null)
        {
            StackCell stackCell = stackItem.GetStackCell();

            PlaySound(trashBinSound);
            stackItem.ReduceStack(limit);

            StackGroupEventDescriptor desc = new StackGroupEventDescriptor();
            desc.sourceGroup = this;
            desc.destinationGroup = this;
            desc.sourceCell = stackCell;
            desc.destinationCells.Add(stackCell);
            SendNotification(desc);
        }
    }

    /// <summary>
    /// Distribute destination item in source group than place source item in just vacated cell.
    /// </summary>
    /// <returns><c>true</c>, if item was replaced, <c>false</c> otherwise.</returns>
    /// <param name="sourceStackItem">Source stack item.</param>
    /// <param name="destinationStackItem">Destination stack item.</param>
    public bool ReplaceItems(StackItem sourceStackItem, StackItem destinationStackItem)
    {
        bool res = false;
        StackCell sourceStackCell = sourceStackItem.GetStackCell();
        StackCell destinationStackCell = destinationStackItem.GetStackCell();
        StackGroup sourceStackGroup = AccessUtility.GetComponentInParent<StackGroup>(sourceStackItem.transform);
        if (sourceStackItem != null && destinationStackItem != null && sourceStackCell != null && destinationStackCell != null && sourceStackGroup != null)
        {
            StackGroupEventDescriptor distributeDesc = new StackGroupEventDescriptor();
            StackGroupEventDescriptor swapDesc = new StackGroupEventDescriptor();

            // Try distribute item from destination cell into source group
            DistributeResults distributeResults = sourceStackGroup.DistributeAnywhere(destinationStackItem, destinationStackItem.GetStack(), null);

            if (distributeResults.amount > 0)
            {
                distributeDesc.sourceGroup = this;
                distributeDesc.destinationGroup = sourceStackGroup;
                distributeDesc.sourceCell = destinationStackCell;
                distributeDesc.destinationCells = distributeResults.cells;

                // If destination cell is empty now
                if (destinationStackCell.GetStackItem() == null)
                {
                    // Place source item into it
                    if (destinationStackCell.UniteStack(sourceStackItem, sourceStackItem.GetStack()) > 0)
                    {
                        PlaySound(sourceStackItem.sound);

                        swapDesc.sourceGroup = sourceStackGroup;
                        swapDesc.destinationGroup = this;
                        swapDesc.sourceCell = sourceStackCell;
                        swapDesc.destinationCells.Add(destinationStackCell);

                        res = true;
                    }
                }
            }

            if (distributeResults.amount > 0)
            {
                // Send distribute stack event notification
                SendNotification(distributeDesc);
                if (res == true)
                {
                    // Send swap stack item event notification
                    SendNotification(swapDesc);
                }
            }

        }
        return res;
    }

    /// <summary>
    /// Gets the allowed space for specified item.
    /// </summary>
    /// <returns>The allowed space.</returns>
    /// <param name="stackItem">Stack item.</param>
    public int GetAllowedSpace(StackItem stackItem)
    {
        double res = 0;
        if (stackItem != null)
        {
            foreach (StackCell stackCell in GetComponentsInChildren<StackCell>(true))
            {
                StackItem item = stackCell.GetStackItem();
                if (item != null)
                {
                    if (stackCell.HasSameItem(stackItem) == true)
                    {
                        res += stackCell.GetAllowedSpace();
                    }
                }
                else
                {
                    res += stackCell.GetAllowedSpace();
                }
            }
        }
        if (res > int.MaxValue)
        {
            res = int.MaxValue;
        }
        return (int)res;
    }

    /// <summary>
    /// Gets the free stack cells.
    /// </summary>
    /// <returns>The free stack cells.</returns>
    /// <param name="stackItem">Stack item.</param>
    public List<StackCell> GetFreeStackCells(StackItem stackItem)
    {
        List<StackCell> res = new List<StackCell>();
        if (stackItem != null)
        {
            foreach (StackCell stackCell in GetComponentsInChildren<StackCell>(true))
            {
                if (stackCell.GetStackItem() == null)
                {
                    if (SortCell.IsSortAllowed(stackCell.gameObject, stackItem.gameObject) == true)
                    {
                        res.Add(stackCell);
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Gets the similar stack items (with same sort).
    /// </summary>
    /// <returns>The similar stack items.</returns>
    /// <param name="stackItem">Stack item.</param>
    public List<StackItem> GetSimilarStackItems(StackItem stackItem)
    {
        List<StackItem> res = new List<StackItem>();
        if (stackItem != null)
        {
            foreach (StackCell stackCell in GetComponentsInChildren<StackCell>(true))
            {
                StackItem sameStackItem = stackCell.GetStackItem();
                if (sameStackItem != null)
                {
                    if (SortCell.IsSortAllowed(stackCell.gameObject, stackItem.gameObject) == true)
                    {
                        res.Add(sameStackItem);
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Gets list of cells with similar stack items.
    /// </summary>
    /// <returns>The cells with similar stack items.</returns>
    /// <param name="stackItem">Stack item.</param>
    public List<StackCell> GetSimilarStackCells(StackItem stackItem)
    {
        List<StackCell> res = new List<StackCell>();
        foreach (StackItem similarStackItem in GetSimilarStackItems(stackItem))
        {
            res.Add(similarStackItem.GetStackCell());
        }
        return res;
    }

    /// <summary>
    /// Raises the DaD group event.
    /// </summary>
    /// <param name="desc">Desc.</param>
    public void OnDadGroupEvent(DadCell.DadEventDescriptor desc)
    {
        switch (desc.triggerType)
        {
            case DadCell.TriggerType.DragGroupRequest:
            case DadCell.TriggerType.DropGroupRequest:
                if (myState == MyState.WaitForRequest)
                {
                    // Disable standard DaD logic
                    desc.groupPermission = false;
                    myState = MyState.WaitForEvent;
                }
                break;
            case DadCell.TriggerType.DragEnd:
                if (myState == MyState.WaitForEvent)
                {
                    StackGroup sourceStackControl = AccessUtility.GetComponentInParent<StackGroup>(desc.sourceCell.transform);
                    StackGroup destStackControl = AccessUtility.GetComponentInParent<StackGroup>(desc.destinationCell.transform);
                    if (sourceStackControl != destStackControl)
                    {
                        // If this group is source group - do nothing
                        myState = MyState.WaitForRequest;
                    }
                }
                break;
            case DadCell.TriggerType.DropEnd:
                if (myState == MyState.WaitForEvent)
                {
                    // If this group is destination group
                    myState = MyState.Busy;
                    // Operate item's drop
                    eventHandkerCoroutine = coroutineContainer.StartCoroutine(EventHandler(desc));
                }
                break;
        }
    }

    /// <summary>
    /// Try to distribute item in similar items inside group.
    /// </summary>
    /// <returns>Distribute result.</returns>
    /// <param name="stackItem">Stack item.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="reservedStackCell">Reserved stack cell, excluded from calculation.</param>
    private DistributeResults DistributeInItems(StackItem stackItem, int amount, StackCell reservedStackCell)
    {
        DistributeResults res = new DistributeResults();

        if (stackItem != null)
        {
            if (amount > 0)
            {
                foreach (StackCell stackCell in GetComponentsInChildren<StackCell>(true))
                {
                    if (stackCell != reservedStackCell)
                    {
                        if (stackCell.HasSameItem(stackItem) == true)
                        {
                            int unitedPart = stackCell.UniteStack(stackItem, amount);
                            if (unitedPart > 0)
                            {
                                res.amount += unitedPart;
                                res.cells.Add(stackCell);
                                amount -= unitedPart;
                            }
                        }
                        if (amount <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Try to distribute item in free cells inside group.
    /// </summary>
    /// <returns>Distribute result.</returns>
    /// <param name="stackItem">Stack item.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="reservedStackCell">Reserved stack cell, excluded from calculation.</param>
    private DistributeResults DistributeInCells(StackItem stackItem, int amount, StackCell reservedStackCell)
    {
        DistributeResults res = new DistributeResults();

        if (stackItem != null)
        {
            if (amount > 0)
            {
                foreach (StackCell emptyStackCell in GetFreeStackCells(stackItem))
                {
                    if (emptyStackCell != reservedStackCell)
                    {
                        int unitedPart = emptyStackCell.UniteStack(stackItem, amount);
                        if (unitedPart > 0)
                        {
                            res.amount += unitedPart;
                            res.cells.Add(emptyStackCell);
                            amount -= unitedPart;
                        }
                        if (amount <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Try to distribute between items than between free cells.
    /// </summary>
    /// <returns>Distribute result.</returns>
    /// <param name="stackItem">Stack item.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="reservedStackCell">Reserved stack cell, excluded from calculation.</param>
    private DistributeResults DistributeAnywhere(StackItem stackItem, int amount, StackCell reservedStackCell)
    {
        DistributeResults res = new DistributeResults();
        res = DistributeInItems(stackItem, amount, reservedStackCell);
        amount -= res.amount;
        if (amount > 0)
        {
            DistributeResults distributeInCells = new DistributeResults();
            distributeInCells = DistributeInCells(stackItem, amount, reservedStackCell);
            if (distributeInCells.amount > 0)
            {
                res.amount += distributeInCells.amount;
                foreach (StackCell cell in distributeInCells.cells)
                {
                    res.cells.Add(cell);
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Stack event handler.
    /// </summary>
    /// <returns>The handler.</returns>
    /// <param name="desc">Desc.</param>
    private IEnumerator EventHandler(DadCell.DadEventDescriptor dadDesc)
    {
        StackGroup sourceStackGroup = AccessUtility.GetComponentInParent<StackGroup>(dadDesc.sourceCell.transform);
        StackGroup destStackGroup = AccessUtility.GetComponentInParent<StackGroup>(dadDesc.destinationCell.transform);

        if (sourceStackGroup == null || destStackGroup == null)
        {
            dadDesc.groupPermission = false;
            myState = MyState.WaitForRequest;
            yield break;
        }

        StackCell destStackCell = dadDesc.destinationCell.GetComponent<StackCell>();
        StackCell sourceStackCell = dadDesc.sourceCell.GetComponent<StackCell>();

        if (destStackCell == null || sourceStackCell == null)
        {
            dadDesc.groupPermission = false;
            myState = MyState.WaitForRequest;
            yield break;
        }

        StackItem destStackItem = destStackCell.GetStackItem();
        StackItem sourceStackItem = sourceStackCell.GetStackItem();

        StackGroupEventDescriptor stackDescPrimary = new StackGroupEventDescriptor();   // Stack event info
        stackDescPrimary.sourceGroup = sourceStackGroup;
        stackDescPrimary.destinationGroup = destStackGroup;
        stackDescPrimary.sourceCell = sourceStackCell;

        StackGroupEventDescriptor stackDescSecondary = new StackGroupEventDescriptor(); // One more stack event info in case if destination cell is not empty and items were swapped
        stackDescSecondary.sourceGroup = destStackGroup;
        stackDescSecondary.destinationGroup = sourceStackGroup;
        stackDescSecondary.sourceCell = destStackCell;

        DistributeResults distributeResults = new DistributeResults();                  // Info with results of stack item distribution in stack group

        PriceItem priceItem = sourceStackItem.GetComponent<PriceItem>();
        PriceGroup buyer = AccessUtility.GetComponentInParent<PriceGroup>(dadDesc.destinationCell.transform);
        PriceGroup seller = AccessUtility.GetComponentInParent<PriceGroup>(dadDesc.sourceCell.transform);

        AudioClip itemSound = sourceStackItem.sound;                                    // Item's SFX

        int amount = sourceStackItem.GetStack();                                        // Item's stack amount

        if (amount > 1)
        {
            // If item's stack > 1 try to use split interface
            if ((globalSplit == true)
                || (sourceStackGroup != destStackGroup && (sourceStackGroup.splitOuter == true || destStackGroup.splitOuter == true)))
            {
                // Need to use split interface
                if (splitInterface != null)
                {
                    if (priceItem != null && buyer != null && seller != null && buyer != seller)
                    {
                        // Split with prices
                        splitInterface.ShowSplitter(sourceStackItem, priceItem);
                    }
                    else
                    {
                        // Split without prices
                        splitInterface.ShowSplitter(sourceStackItem, null);
                    }
                    // Show split interface and wait while it is active
                    while (splitInterface.gameObject.activeSelf == true)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    // Get splitted stack amount
                    amount = splitInterface.GetRightAmount();
                }
            }
        }

        if (amount > 0)
        {
            if (sourceStackGroup != destStackGroup
                && (destStackGroup.arrangeMode == true || sourceStackGroup.arrangeMode == true))
            {
                // Split in arrange mode between different stack groups
                if (priceItem != null && buyer != null && seller != null && buyer != seller)
                {
                    // Different price groups
                    if ((long)buyer.GetCash() >= (long)priceItem.GetPrice() * amount)
                    {
                        // Has anough cash
                        distributeResults = DistributeAnywhere(sourceStackItem, amount, null);
                        if (distributeResults.amount > 0)
                        {
                            stackDescPrimary.destinationCells = distributeResults.cells;

                            int totalPrice = priceItem.GetPrice() * distributeResults.amount;
                            seller.AddCash(totalPrice);
                            buyer.SpendCash(totalPrice);

                            buyer.UpdatePrices();
                        }
                    }
                }
                else
                {
                    // Same price group
                    distributeResults = DistributeAnywhere(sourceStackItem, amount, null);
                    if (distributeResults.amount > 0)
                    {
                        stackDescPrimary.destinationCells = distributeResults.cells;
                    }
                }
            }
            else
            {
                // Inside same stack group transactions disabled in arrange mode
                if (arrangeMode == false)
                {
                    if (destStackItem != null)
                    {
                        // Check if items allowed for destination cell
                        if (SortCell.IsSortAllowed(destStackCell.gameObject, sourceStackItem.gameObject) == true)

                        {
                            // Destination cell already has same item
                            if (destStackCell.HasSameItem(sourceStackItem) == true)
                            {
                                if (destStackCell.UniteStack(sourceStackItem, amount) > 0)
                                {
                                    stackDescPrimary.destinationCells.Add(destStackCell);
                                }
                            }
                            // Check if items allowed for source cell
                            else if (SortCell.IsSortAllowed(sourceStackCell.gameObject, destStackItem.gameObject) == true)
                            {
                                // Different items. Try to swap items between cells
                                if (destStackCell.SwapStacks(sourceStackCell) == true)
                                {
                                    // Swap successful
                                    stackDescSecondary.destinationCells.Add(sourceStackCell);
                                    sourceStackItem = sourceStackCell.GetStackItem();
                                    if (sourceStackItem != null)
                                    {
                                        // Distribute item after swap
                                        distributeResults = DistributeInItems(sourceStackItem, sourceStackItem.GetStack(), destStackCell);
                                        if (distributeResults.amount > 0)
                                        {
                                            stackDescPrimary.destinationCells = distributeResults.cells;
                                        }
                                        if (destStackCell.GetStackItem() != null)
                                        {
                                            // If stack item (or part of it) in destination cell
                                            stackDescPrimary.destinationCells.Add(destStackCell);
                                        }
                                    }
                                }
                                else
                                {
                                    // Swap unsuccessful.
                                    // Try to distribute item between other cells to make cell free
                                    distributeResults = DistributeAnywhere(destStackItem, destStackItem.GetStack(), destStackCell);
                                    if (distributeResults.amount > 0)
                                    {
                                        stackDescSecondary.destinationCells = distributeResults.cells;
                                    }
                                    destStackItem = destStackCell.GetStackItem();
                                    if (destStackItem != null)
                                    {
                                        // Item still in cell. Try to place item in other group's cells
                                        distributeResults = sourceStackGroup.DistributeAnywhere(destStackItem, destStackItem.GetStack(), null);
                                        if (distributeResults.amount > 0)
                                        {
                                            stackDescSecondary.destinationCells.AddRange(distributeResults.cells);
                                        }
                                        destStackItem = destStackCell.GetStackItem();
                                        if (destStackItem == null)
                                        {
                                            // Item was placed into other cell and now this cell is free
                                            // Place item into destination cell
                                            if (destStackCell.UniteStack(sourceStackItem, amount) > 0)
                                            {
                                                stackDescPrimary.destinationCells.Add(destStackCell);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Item was placed into other cell and now this cell is free
                                        // Place item into destination cell
                                        if (destStackCell.UniteStack(sourceStackItem, amount) > 0)
                                        {
                                            stackDescPrimary.destinationCells.Add(destStackCell);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Destination cell has no item
                        // Place item into destination cell
                        if (destStackCell.UniteStack(sourceStackItem, amount) > 0)
                        {
                            stackDescPrimary.destinationCells.Add(destStackCell);
                        }
                    }
                }
            }
        }

        // Send stack event notifications
        if (stackDescSecondary.destinationCells.Count > 0)
        {
            SendNotification(stackDescSecondary);
        }
        if (stackDescPrimary.destinationCells.Count > 0)
        {
            SendNotification(stackDescPrimary);
            if (trashBinMode == true)
            {
                // In trash bin mode just destroy item
                dadDesc.destinationCell.RemoveItem();
                PlaySound(trashBinSound);
            }
            else
            {
                PlaySound(itemSound);
            }
        }

        myState = MyState.WaitForRequest;
        eventHandkerCoroutine = null;
    }

    /// <summary>
    /// Sends the stack event notification.
    /// </summary>
    /// <param name="desc">Event descriptor.</param>
    private void SendNotification(StackGroupEventDescriptor desc)
    {
        if (desc.sourceGroup != null)
        {
            // Send notification to source GO
            AccessUtility.SendMessageUpwards(desc.sourceGroup.transform, "OnStackGroupEvent", desc);
            foreach (GameObject receiver in desc.sourceGroup.eventAdditionalReceivers)
            {
                // Send notification to additionaly specified GOs
                AccessUtility.SendMessage(receiver.transform, "OnStackGroupEvent", desc);
            }
        }
        if (desc.destinationGroup != null && desc.sourceGroup != desc.destinationGroup && desc.destinationGroup.trashBinMode == false)
        {
            // Send notification to destination GO
            AccessUtility.SendMessageUpwards(desc.destinationGroup.transform, "OnStackGroupEvent", desc);
            foreach (GameObject receiver in desc.destinationGroup.eventAdditionalReceivers)
            {
                // Send notification to additionaly specified GOs
                AccessUtility.SendMessage(receiver.transform, "OnStackGroupEvent", desc);
            }
        }

    }

    /// <summary>
    /// Plaies the sound.
    /// </summary>
    /// <param name="sound">Sound.</param>
    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            if (audioSource == null)
            {
                audioSource = Camera.main.GetComponent<AudioSource>();
            }
            if (audioSource != null)
            {
                audioSource.PlayOneShot(sound);
            }
        }
    }

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    void OnDestroy()
    {
        if (coroutineContainer != null && eventHandkerCoroutine != null)
        {
            coroutineContainer.StopCoroutine(eventHandkerCoroutine);
        }
    }
}
