using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;

public class ClickController : MonoBehaviour
{
    [Header("Referencias de Selección")]
    public RectTransform selectionBoxUI; 
    private Rect selectionRect;          
    private Vector2 startMousePos;       
    private bool isDragging = false;     

    [Header("Configuración")]
    public KeyCode multiSelectKey = KeyCode.LeftShift; 

    void Start()
    {
        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSelectionBox();
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateSelectionBox();
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndSelectionBox();
        }
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleRightClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Resource"))
        {
            Resource targetResource = hit.collider.GetComponent<Resource>();
            if (targetResource != null)
            {
                UnitSelector[] allUnits = FindObjectsByType<UnitSelector>(FindObjectsSortMode.None);
                foreach (UnitSelector unit in allUnits)
                {
                    if (unit.isSelected)
                    {
                        Collector collector = unit.GetComponent<Collector>();
                        if (collector != null)
                        {
                            collector.StartCollecting(hit.transform);
                        }
                    }
                }
                return;
            }
        }
        MoveSelectedUnits();
    }

    private void StartSelectionBox()
    {
        if (!Input.GetKey(multiSelectKey))
        {
            DeselectAllUnits();
        }
        startMousePos = Input.mousePosition;
        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(true);
        }
        isDragging = true;
    }

    private void UpdateSelectionBox()
    {
        if (selectionBoxUI == null) return;
        float width = Input.mousePosition.x - startMousePos.x;
        float height = Input.mousePosition.y - startMousePos.y;
        Vector2 center = startMousePos + new Vector2(width / 2f, height / 2f);
        selectionBoxUI.position = center;
        selectionBoxUI.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        float xMin = Mathf.Min(startMousePos.x, Input.mousePosition.x);
        float xMax = Mathf.Max(startMousePos.x, Input.mousePosition.x);
        float yMin = Mathf.Min(startMousePos.y, Input.mousePosition.y);
        float yMax = Mathf.Max(startMousePos.y, Input.mousePosition.y);
        selectionRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private void EndSelectionBox()
    {
        if (selectionBoxUI != null)
        {
            selectionBoxUI.gameObject.SetActive(false);
        }
        if (selectionRect.width < 2 && selectionRect.height < 2)
        {
            SelectSingleUnitByClick();
        }
        else
        {
            SelectUnitsInRect();
        }
        isDragging = false;
    }

    private void SelectSingleUnitByClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (!Input.GetKey(multiSelectKey))
        {
            DeselectAllUnits();
        }
        if (hit.collider != null)
        {
            UnitSelector selector = hit.collider.GetComponent<UnitSelector>();
            if (selector != null)
            {
                selector.SetSelected(true);
            }
        }
    }

    private void SelectUnitsInRect()
    {
        UnitSelector[] allUnits = FindObjectsByType<UnitSelector>(FindObjectsSortMode.None);
        if (!Input.GetKey(multiSelectKey))
        {
            DeselectAllUnits();
        }
        Camera cam = Camera.main;
        foreach (UnitSelector unit in allUnits)
        {
            Vector2 unitScreenPos = cam.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(unitScreenPos))
            {
                unit.SetSelected(true);
            }
        }
    }

    private void DeselectAllUnits()
    {
        UnitSelector[] allUnits = FindObjectsByType<UnitSelector>(FindObjectsSortMode.None);
        foreach (UnitSelector unit in allUnits)
        {
            unit.SetSelected(false);
        }
    }

    void MoveSelectedUnits()
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0;

        UnitSelector[] allUnits = FindObjectsByType<UnitSelector>(FindObjectsSortMode.None);
        foreach (UnitSelector unit in allUnits)
        {
            if (unit.isSelected)
            {
                // Limpieza para el recolector: si se manda a mover, dejamos de recolectar
                Collector col = unit.GetComponent<Collector>();
                if (col != null) col.currentState = 0;

                UnitMovement movement = unit.GetComponent<UnitMovement>();
                if (movement != null)
                {
                    movement.SetNewDestination(targetPos);
                }
            }
        }
    }
}