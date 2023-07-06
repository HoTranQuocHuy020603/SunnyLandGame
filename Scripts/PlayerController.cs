using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Start() Variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    //FSM
    private enum State {idle, running, jumping, falling, hurt}
    private State state = State.idle;

    //Inspector Variables
    [SerializeField]private int damge = 1;
    [SerializeField]private LayerMask ground;
    [SerializeField]private float speed = 5f;
    [SerializeField]private float jumpForce = 7f;
    [SerializeField]private float hurtForce = 5f;
    [SerializeField]private AudioSource cherry;
    [SerializeField]private AudioSource footstep;
    [SerializeField]private AudioSource death;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        death = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state",(int)state);//thiet lap hoat anh dua tren trang thai duoc liet ke
        if(Input.GetKey(KeyCode.B))
        {
            HelpHealth();
        }  
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            cherry.Play();
            Destroy(collision.gameObject);
            PermanentUi.perm.cherries += 1;
            PermanentUi.perm.cherryText.text = PermanentUi.perm.cherries.ToString();
        }
        if(collision.tag == "Powerup")//An gem de nang cap toc do va do nhay cao trong 10s
        {
            Destroy(collision.gameObject);
            speed = 7f;
            jumpForce = 10f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }
        if(collision.tag == "BackLvDia")//An gem neu muon quay ve lv 3 :D
        {
            Destroy(collision.gameObject);
            HandleHealth();
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                HandleHealth();// Tru mau nhan vat va reset level khi mau nho hon 0
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Ke dich o phia ben phai, player co the bi thuong va di chuyen sang trai
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Ke dich o phia ben trai, player co the bi thuong va di chuyen sang phai
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            } 
        }
    }
    private void HandleHealth()
    {
        PermanentUi.perm.health -= damge;
        PermanentUi.perm.healthBar.fillAmount = PermanentUi.perm.health / PermanentUi.perm.healthMax;   
        if(PermanentUi.perm.health <= 0)
        {
            anim.SetTrigger("Death");
            death.Play();
            PermanentUi.perm.Resett();          
            SceneManager.LoadScene("Lv1");  
        }
    }
    private void HelpHealth()
    {
            if(PermanentUi.perm.cherries >= 5 && PermanentUi.perm.health <= 14 )
            {
                PermanentUi.perm.health += 3;
                PermanentUi.perm.healthBar.fillAmount = PermanentUi.perm.health / PermanentUi.perm.healthMax;
                PermanentUi.perm.cherries -=  5;
                PermanentUi.perm.cherryText.text = PermanentUi.perm.cherries.ToString();
            }
    }

    private void  Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        //Moving left
        if(hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        //Moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        //Jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {

        if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }     
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    private void Footstep()
    {
        footstep.Play();
    }

    private IEnumerator ResetPower()
    {
        yield return  new WaitForSeconds(10);
        speed = 5f;
        jumpForce = 7f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    private void Death()
    {
        Destroy(this.gameObject);
    }
}
