using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundedLayerMask;
    public float maxJumpTime = 0.5f;
    public float runSpeed = 7f;
    public float jumpHeight = 4.5f;
    public float extraJump = 12f;
    public float maxSpeedRunning = 6f;
    public float groundedDistance = 0.05f;
    public float gravity = 6f;


    private bool playerJumping;
    private bool playerMoving;
    private float jumpTime;
    private Vector2 velocity;
    private Rigidbody2D rb;
    BoxCollider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = rb.velocity;
        playerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;
        UpdateInput();
    }


    /// <summary>
    /// Actualiza el movimiento del personaje según la tecla pulsada.
    /// </summary>
    private void UpdateInput()
    {
        //INPUT MOVIMIENTOS
        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerMoving = true;
            RightMovement();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerMoving = true;
            LeftMovement();
        }
        else
            playerMoving = false;


        if (Input.GetButtonDown("Jump") && GroundDetection())
        {
            playerJumping = true;
            jumpTime = maxJumpTime;
            Jump();
        }

        if (Input.GetButtonUp("Jump") || jumpTime <= 0 || CeilingCollision())
        {
            playerJumping = false;
        }

        if (playerJumping)
        {
            jumpTime -= Time.deltaTime;

            if (jumpTime > 0)
                rb.AddForce(Vector2.up * extraJump);
        }
        

        if (!GroundDetection() && !playerJumping)
            rb.velocity -= new Vector2(0, gravity) * Time.deltaTime;


        if (playerJumping || playerMoving)
            rb.drag = 0f;
        else
            rb.drag = 3f;
    }


    /// <summary>
    /// Movimiento hacia la derecha
    /// </summary>
    private void RightMovement()
    {
        if (GroundDetection())
        {
            if (velocity.x < maxSpeedRunning)
            {
                rb.velocity += new Vector2(transform.right.x * runSpeed, transform.right.y * runSpeed) * Time.deltaTime;    
            }
            else
            {
                velocity.x = maxSpeedRunning;
                rb.velocity = new Vector2(velocity.x, velocity.y);
            }
        }
        else
        {
            if (velocity.x < maxSpeedRunning)
                rb.velocity += new Vector2(transform.right.x * runSpeed, transform.right.y * runSpeed) * Time.deltaTime;
            else
            {
                velocity.x = maxSpeedRunning;
                rb.velocity = new Vector2(velocity.x, velocity.y);
            }
        }

    }

    /// <summary>
    /// Movimiento hacia la izquierda.
    /// </summary>
    private void LeftMovement()
    {
        if (GroundDetection())
        {
            if (velocity.x > -maxSpeedRunning)
            {
                rb.velocity -= new Vector2(transform.right.x * runSpeed, transform.right.y * runSpeed) * Time.deltaTime;
            }
            else
            {
                velocity.x = -maxSpeedRunning;
                rb.velocity = new Vector2(velocity.x, velocity.y);
            }
        }
        else
        {
            if (velocity.x > -maxSpeedRunning)
                rb.velocity -= new Vector2(transform.right.x * runSpeed, transform.right.y * runSpeed) * Time.deltaTime;
            else
            {
                velocity.x = -maxSpeedRunning;
                rb.velocity = new Vector2(velocity.x, velocity.y);
            }
        }
    }

    /// <summary>
    /// Salto.
    /// </summary>
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Realiza 3 raycast hacia abajo para detectar una superficie por la que caminar.
    /// </summary>
    bool GroundDetection()
    {
        RaycastHit2D rightHit;
        RaycastHit2D leftHit;
        RaycastHit2D midHit;
        float rate = playerCollider.size.x / 2;
        Debug.DrawRay(new Vector2(transform.position.x + rate, transform.position.y - rate), Vector2.down, Color.green);
        Debug.DrawRay(new Vector2(transform.position.x - rate, transform.position.y - rate), Vector2.down, Color.green);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rate), Vector2.down, Color.green);

        rightHit = Physics2D.Raycast(new Vector2(transform.position.x + rate, transform.position.y - rate), Vector2.down, groundedDistance, groundedLayerMask);
        leftHit = Physics2D.Raycast(new Vector2(transform.position.x - rate, transform.position.y - rate), Vector2.down, groundedDistance, groundedLayerMask);
        midHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rate), Vector2.down, groundedDistance, groundedLayerMask);

        if (midHit.collider != null || leftHit.collider != null || rightHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Realiza 3 raycast hacia arriba para detectar si colisiona con un bloque; en cuyo caso 
    /// llama a la función de Btoe (BlockBounce) de dicho bloque.
    /// </summary>
    bool CeilingCollision()
    {
        RaycastHit2D rightHit;
        RaycastHit2D leftHit;
        RaycastHit2D midHit;
        Vector2 head = new Vector2(transform.position.x, transform.position.y);
        float width = playerCollider.size.x / 2;
        float height = playerCollider.size.y / 2;

        Debug.DrawRay(new Vector2(head.x + width, head.y + height), Vector2.up, Color.red);
        Debug.DrawRay(new Vector2(head.x - width, head.y + height), Vector2.up, Color.red);
        Debug.DrawRay(new Vector2(head.x, head.y + height), Vector2.up, Color.red);

        rightHit = Physics2D.Raycast(new Vector2(head.x + width, head.y + height), Vector2.up, groundedDistance, groundedLayerMask);
        leftHit = Physics2D.Raycast(new Vector2(head.x - width, head.y + height), Vector2.up, groundedDistance, groundedLayerMask);
        midHit = Physics2D.Raycast(new Vector2(head.x, head.y + height), Vector2.up, groundedDistance, groundedLayerMask);


        if (midHit.collider != null || leftHit.collider != null || rightHit.collider != null)
        {
            RaycastHit2D hitRay = rightHit;

            if (rightHit)
                hitRay = rightHit;
            else if (midHit)
                hitRay = midHit;
            else if (leftHit)
                hitRay = leftHit;

            return true;
        }
        else
            return false;
    }


}
