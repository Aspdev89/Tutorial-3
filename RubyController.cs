using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Uncharted 4 is the best video game ever made
// if (Input.GetKey("escape")){  Application.Quit();  }

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public int cogs = 4;

    public static int level;

    public AudioClip winMusic;
    public AudioClip loseMusic;
    public AudioSource musicSource;

    public int score = 0;
    public int count = 0;
    public Text gameOverText;
    public Text ammoText;
    public Text scoreText;
    public Text jambiText;
    public bool gameOver;

    public GameObject projectilePrefab;

    public GameObject explosionPrefab;
    public GameObject starsPrefab;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update

    AudioSource audioSource;
    public AudioClip collectedClip;
    public AudioClip collectedClip2;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        scoreText.text = score.ToString();
        gameOverText.text = "";
        ammoText.text = cogs.ToString();
        gameOver = false;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) { Application.Quit(); }
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));

            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();

                if (character != null)
                {
                    character.DisplayDialog();
                }
                if (level != 2)
                {
                    if (count == 4)
                    {
                        level = 2;
                        SceneManager.LoadScene("Main2");
                    } //make this teleport player to stage 2
                }
            }
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogs > 0)
            {
                Launch();
                cogs -= 1;
                ammoText.text = cogs.ToString();
                audioSource.PlayOneShot(collectedClip);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            GameObject explosionObject = Instantiate(explosionPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            audioSource.PlayOneShot(collectedClip2);
            animator.SetTrigger("Hit");
        }
        if (amount > 0)
        {
            GameObject starsObject = Instantiate(starsPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            gameOver = true;
            speed = 0.0f;
        }
        if (gameOver == true)
        {
            gameOverText.text = "You Lose! Restart?";
            musicSource.Stop();
            musicSource.clip = loseMusic;

            musicSource.Play();
        }
    }

    public void ChangeScore(int score)
    {

        print("LMFAO");
        //score += 1;
        count += 1;
        scoreText.text = count.ToString();

        if (level != 2)
        {
            if (count == 4)
            {
                jambiText.text = "Jambi: Thanks for bringing me the horizon! See me to reach the next level!";
            }
        }
        if (level != 0)
        {
            if (count == 4)
            {
                gameOver = true;
                //count += 1; count = 9;
                //scoreText.text = count.ToString();
            }
            if (gameOver == true)
            {
                gameOverText.text = "You Win! Game by Joey DeSimone";
                musicSource.Stop();
                musicSource.clip = winMusic;

                musicSource.Play();
            }
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ammo")
        {
            cogs += 4;
            ammoText.text = cogs.ToString();
            Destroy(collision.collider.gameObject);
            PlaySound(collectedClip);
        }
    }
}