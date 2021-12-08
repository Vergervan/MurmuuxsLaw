using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

using NetworkType = NetworkManager.NetworkType;
using static PlayerInfo.Types;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private NetworkManager network;
    public Guid playerGuid;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isMoving = false;
    private bool isOnline = false;
    private Vector2 oldPos;
    public enum AnimationState
    {
        Idle = 0,
        Walk = 1,
        WalkBack = 2,
        IdleBack = 3
    }
    public float speed;
    private AnimationState state = AnimationState.Idle;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isOnline) return;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "ItemImage")
                {
                    if (Vector2.Distance(transform.position, hit.collider.transform.position) < 1.5f)
                    {
                        //Debug.Log("You can take item " + hit.collider.GetComponent<LevelItem>().item.ToString());
                        inventory.AddItemToInventory(hit.collider.GetComponent<LevelItem>().item);
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (isOnline) return;
        Vector2Int direction = ReadMovementKeys();
        if (direction.magnitude > 0) isMoving = true;
        else
        {
            if (!isMoving) return;
            isMoving = false;
            if (network.networkType == NetworkType.Client)
            {
                SendPositionToServer(Vector2Int.zero);
                return;
            }
        }
        switch (network.networkType) 
        {
            case NetworkType.Client:
                SendPositionToServer(direction);
                break;
            case NetworkType.Server:
                network.SendHostPlayerMovement(direction);
                goto default;
            default:
                MoveLocal(direction);
                break;
        }
    }

    private Vector2Int ReadMovementKeys()
    {
        Vector2Int direction = Vector2Int.zero;
        if (Input.GetKey(KeyCode.A))
            direction -= Vector2Int.right;
        if (Input.GetKey(KeyCode.D))
            direction += Vector2Int.right;
        if (Input.GetKey(KeyCode.W))
            direction += Vector2Int.up;
        if (Input.GetKey(KeyCode.S))
            direction -= Vector2Int.up;
        return direction;
    }

    public void MoveLocal(Vector2 direction)
    {
        Vector2 newScale = transform.localScale;
        newScale.x *= ((direction.x < 0 && newScale.x < 0) || (direction.x > 0 && newScale.x > 0)) ? -1 : 1;
        transform.localScale = newScale;

        //transform.Translate(direction * speed * Time.deltaTime);
        rb.velocity = direction * speed * Time.fixedDeltaTime;
        if (direction.y > 0) state = AnimationState.WalkBack;
        else if (direction.magnitude > 0 || direction.y < 0) state = AnimationState.Walk;
        else if (direction.y == 0)
        {
            if (state == AnimationState.WalkBack) state = AnimationState.IdleBack;
            else if (state == AnimationState.Walk) state = AnimationState.Idle;
        }
        animator.SetInteger("CurrentAnim", (int)state);
        //if (isOnline)
        //{
        //    if(Vector2.Distance(oldPos, transform.position) > 0.001f)
        //    {
        //        PlayerInfo info = new PlayerInfo
        //        {
        //            Type = PlayerInfo.Types.MessageType.AcceptMove,
        //            Direction = new PlayerInfo.Types.vec2 { X = (int)direction.x, Y = (int)direction.y }
        //        };
        //        network.SendMessageTo(playerGuid, info);
        //    }
        //}
    }
    private void SendPositionToServer(Vector2Int direction)
    {
        network.SendMessageTo(network.Socket, new PlayerInfo
        {
            Type = MessageType.Move,
            Direction = new vec2 { X = direction.x, Y = direction.y },
            Position = new vec2f { X = transform.position.x, Y = transform.position.y },
            Guid = playerGuid.ToString("N")
        });
    }
    public void SetIsOnlinePlayer(bool b) => isOnline = b;
    public void SetNetworkManager(NetworkManager nm) => network = nm;
    public void RememberOldPos() => oldPos = transform.position;
}
