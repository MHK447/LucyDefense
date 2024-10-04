using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isUnitSelected;
    private GameObject selectedUnit;

    private UnitTileComponent FirstSelectComponent;

    private UnitTileComponent SecondSelectComponent;

    private InGameUnitSelect InGameUnitSelectUI;

    private InGameUnitBase SelectAttackArangeUnit;

    void Start()
    {
        mainCamera = Camera.main;

        // LineRenderer 설정
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = 5;

        if (InGameUnitSelectUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameUnitSelect>((unitselect) => {
                ProjectUtility.SetActiveCheck(unitselect.gameObject, false);
                InGameUnitSelectUI = unitselect;
            });
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (IsPointerOverUIObject())
        {
            return;
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (SelectAttackArangeUnit != null)
                SelectAttackArangeUnit.TileAttackRangeActive(false);


            var getunitinfo = GameRoot.Instance.UISystem.GetUI<PopupInGameUnitInfo>();

            if(getunitinfo != null && getunitinfo.gameObject.activeSelf)
            {
                getunitinfo.Hide();
            }

            ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, false);

            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                FirstSelectComponent = hit.collider.GetComponent<UnitTileComponent>();

                if (FirstSelectComponent.UnitList.Count > 0)
                {
                    selectedUnit = hit.collider.gameObject;
                    isUnitSelected = true;
                    FirstSelectComponent.EnableTile();
                }
                else
                {
                    isUnitSelected = false;
                }
            }
        }


        if (isUnitSelected && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 currentMousePosition2D = new Vector2(currentMousePosition.x, currentMousePosition.y);
            RaycastHit2D hit = Physics2D.Raycast(currentMousePosition2D, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                if (SecondSelectComponent != null && FirstSelectComponent != SecondSelectComponent)
                    SecondSelectComponent.DisableTile();

                SecondSelectComponent = hit.collider.GetComponent<UnitTileComponent>();
                currentMousePosition.z = 0;
                DrawPath(selectedUnit.transform.position, hit.transform.position);
                SecondSelectComponent.EnableTile();
            }
        }

        if (isUnitSelected && (Input.GetMouseButtonUp(0) || (Input.touchCount == 0 && Input.GetMouseButtonUp(0))))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

            if (FirstSelectComponent != null && SecondSelectComponent == null)
            {
                //같은곳을 선택할경우
                var finddata = FirstSelectComponent.UnitList.FirstOrDefault();

                if(finddata != null)
                {
                    finddata.TileAttackRangeActive(true);

                    GameRoot.Instance.UISystem.OpenUI<PopupInGameUnitInfo>(popup => popup.Set(finddata.GetUnitIdx));
                    ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, true);
                    InGameUnitSelectUI.Init(FirstSelectComponent.transform);
                    InGameUnitSelectUI.Set(finddata.GetUnitIdx, FirstSelectComponent);


                    FirstSelectComponent.DisableTile();
                    SecondSelectComponent.DisableTile();
                    FirstSelectComponent = null;
                    SecondSelectComponent = null;

                    
                }
            }
            else if (FirstSelectComponent != null && SecondSelectComponent != null)
            {
                if (FirstSelectComponent.GetTileSpawnOrder == SecondSelectComponent.GetTileSpawnOrder)
                {
                    //같은곳을 선택할경우
                    var finddata = FirstSelectComponent.UnitList.FirstOrDefault();

                    if (finddata != null)
                    {
                        SelectAttackArangeUnit = finddata;
                        finddata.TileAttackRangeActive(true);
                        ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, true);
                        InGameUnitSelectUI.Init(FirstSelectComponent.transform);
                        InGameUnitSelectUI.Set(finddata.GetUnitIdx , FirstSelectComponent );
                        GameRoot.Instance.UISystem.OpenUI<PopupInGameUnitInfo>(popup => popup.Set(finddata.GetUnitIdx));

                        FirstSelectComponent.DisableTile();
                        SecondSelectComponent.DisableTile();
                        FirstSelectComponent = null;
                        SecondSelectComponent = null;
                    }
                }
                else
                {
                    FirstSelectComponent.MoveChangeTileUnit(SecondSelectComponent);
                    SecondSelectComponent.MoveChangeTileUnit(FirstSelectComponent);

                    FirstSelectComponent.SetTileUnitIdx(-1);
                    SecondSelectComponent.SetTileUnitIdx(-1);
                    //유닛교체 
                    var tempunitlist = FirstSelectComponent.UnitList.ToList();
                    FirstSelectComponent.UnitList = SecondSelectComponent.UnitList;
                    if (SecondSelectComponent.UnitList.Count > 0)
                    {
                        FirstSelectComponent.SetTileUnitIdx(SecondSelectComponent.UnitList.First().GetUnitIdx);
                    }
                    SecondSelectComponent.UnitList = tempunitlist;
                    if (tempunitlist.Count > 0)
                    {
                        SelectAttackArangeUnit = tempunitlist[0];
                        tempunitlist.First().TileAttackRangeActive(true);
                        SecondSelectComponent.SetTileUnitIdx(tempunitlist.First().GetUnitIdx);
                    }

                    FirstSelectComponent.DisableTile();
                    SecondSelectComponent.DisableTile();
                    FirstSelectComponent = null;
                    SecondSelectComponent = null;
                }
            }
            lineRenderer.positionCount = 0;
        }
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        // 마우스 포인터 위치
        if (Input.touchCount > 0)
        {
            eventDataCurrentPosition.position = Input.GetTouch(0).position;
        }
        else
        {
            eventDataCurrentPosition.position = Input.mousePosition;
        }

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }



    void DrawPath(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    System.Collections.IEnumerator MoveUnit(Vector3 targetPosition)
    {
        float duration = 1.0f; 
        Vector3 startPosition = selectedUnit.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            selectedUnit.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        selectedUnit.transform.position = targetPosition;
        isUnitSelected = false;
        lineRenderer.positionCount = 0;
    }
}