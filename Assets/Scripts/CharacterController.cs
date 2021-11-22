using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    public enum AnimationState
    {
        Idle = 0,
        Walk = 1,
        WalkBack = 2,
        IdleBack = 3
    }
    public float speed;
    private AnimationState state = AnimationState.Idle;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "ItemImage")
                {
                    if(Vector2.Distance(transform.position, hit.collider.transform.position) < 1.5f)
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
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        Vector3 direction = new Vector2();
        if (Input.GetKey(KeyCode.A))
            direction -= Vector3.right;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;
        if (Input.GetKey(KeyCode.W))
            direction += Vector3.up;
        if (Input.GetKey(KeyCode.S))
            direction -= Vector3.up;

        Vector2 newScale = transform.localScale;
        newScale.x *= ((direction.x < 0 && newScale.x < 0) || (direction.x > 0 && newScale.x > 0)) ? -1 : 1;
        transform.localScale = newScale;

        //transform.Translate(direction * speed * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = direction * speed * Time.fixedDeltaTime;
        if (direction.y > 0) state = AnimationState.WalkBack;
        else if (direction.magnitude > 0 || direction.y < 0) state = AnimationState.Walk;
        else if(direction.y == 0)
        {
            if (state == AnimationState.WalkBack) state = AnimationState.IdleBack;
            else if (state == AnimationState.Walk) state = AnimationState.Idle;
        }
        GetComponent<Animator>().SetInteger("CurrentAnim", (int)state);
    }
}
