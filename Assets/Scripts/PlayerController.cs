﻿using System;
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
    private bool isOnline = false;
    private Vector2 direction;
    private bool _isMoving;
    public bool IsMoving { get => _isMoving; }
    public enum AnimationState
    {
        Idle = 0,
        Walk = 1,
        WalkBack = 2,
        IdleBack = 3
    }
    public float speed = 2.7f;
    private AnimationState state = AnimationState.Idle;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isOnline) return;
        TakeItemCheck();
    }
    void FixedUpdate()
    {
        if (isOnline) return;
        direction = ReadMovementKeys();
        if (direction.magnitude > 0)
        {
            switch (network.Type)
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
        
        RefreshAnimation(direction);
    }

    private Vector2 ReadMovementKeys()
    {
        //return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
            direction -= Vector2.right;
        if (Input.GetKey(KeyCode.D))
            direction += Vector2.right;
        if (Input.GetKey(KeyCode.W))
            direction += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            direction -= Vector2.up;
        return direction;
    }

    private bool[] ReadKeysInBools()
    {
        //ADWS
        bool[] keys = new bool[4];

        if (Input.GetKey(KeyCode.A))
            keys[0] = true;
        if (Input.GetKey(KeyCode.D))
            keys[1] = true;
        if (Input.GetKey(KeyCode.W))
            keys[2] = true;
        if (Input.GetKey(KeyCode.S))
            keys[3] = true;
        return keys;
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    private void TakeItemCheck()
    {
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
                        if (inventory.HasFreeCells())
                        {
                            inventory.AddItemToInventory(hit.collider.GetComponent<LevelItem>().item);
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void MoveLocal(Vector2 direction)
    {
        Vector2 newScale = transform.localScale;
        newScale.x *= ((direction.x < 0 && newScale.x < 0) || (direction.x > 0 && newScale.x > 0)) ? -1 : 1;
        transform.localScale = newScale;
        if (direction.magnitude > 0)
        {
            rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
        }
    }
    public void RefreshAnimation(Vector2 direction)
    {
        if (direction.y > 0) state = AnimationState.WalkBack;
        else if (direction.magnitude > 0 || direction.y < 0) state = AnimationState.Walk;
        else if (direction.y == 0)
        {
            if (state == AnimationState.WalkBack) state = AnimationState.IdleBack;
            else if (state == AnimationState.Walk) state = AnimationState.Idle;
        }
        animator.SetInteger("CurrentAnim", (int)state);
    }
    private void SendPositionToServer(Vector2 direction)
    {
        network.SendMessageTo(network.Socket, new PlayerInfo
        {
            Type = MessageType.Move,
            Direction = new vec2f { X = direction.x, Y = direction.y },
            Position = new vec2f { X = transform.position.x, Y = transform.position.y },
            Guid = playerGuid.ToString("N")
        });
    }
    //private void SendPositionToServer(Vector2 direction)
    //{
    //    ProtoPacket packet = new ProtoPacket().WriteShort((short)MessageType.Move).WriteFloat(direction.x).WriteFloat(direction.y);
    //    packet.SendPacket(network.Socket);
    //    Debug.Log("Length: " + packet.Length);
    //}

    //private void SendPositionToServer(bool[] direction)
    //{
    //    ProtoPacket packet = new ProtoPacket().WriteShort((short)MessageType.Move).WriteBool(direction[0]).WriteBool(direction[1]).WriteBool(direction[2]).WriteBool(direction[3]);
    //    packet.SendPacket(network.Socket);
    //    Debug.Log("Length: " + packet.Length);
    //}

    public void SetIsOnlinePlayer(bool b) => isOnline = b;
    public void SetNetworkManager(NetworkManager nm) => network = nm;
}
