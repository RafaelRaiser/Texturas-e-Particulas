using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource danceMusicSource;
    [SerializeField] private ParticleSystem dirtParticlesPrefab; 

    private Animator animator;
    private bool isWalking = false;
    private bool isDancing = false;
    private Rigidbody rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody não encontrado no GameObject. Adicione um Rigidbody.");
        }

        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource não estava configurado, foi adicionado automaticamente.");
        }

        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogError("Nenhum som de passo configurado no array footstepClips.");
        }

        if (danceMusicSource == null)
        {
            Debug.LogError("AudioSource da música da dança não configurado.");
        }

        if (dirtParticlesPrefab == null)
        {
            Debug.LogError("Prefab das partículas de sujeira não configurado.");
        }
    }

    private void Update()
    {
        if (isDancing) return;
        Move();
        MaintainGroundLevel();

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(DanceRoutine());
        }
    }

    private void Move()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (!isWalking)
            {
                isWalking = true;
                animator.SetBool("walk", true);
            }

            if (!footstepAudioSource.isPlaying && footstepClips.Length > 0)
            {
                int randomIndex = Random.Range(0, footstepClips.Length);
                footstepAudioSource.clip = footstepClips[randomIndex];
                footstepAudioSource.Play();

       
                SpawnDirtParticles();
            }

            if (Input.GetKey(KeyCode.W)) movement += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) movement += Vector3.back;
            if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
            if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

            movement = movement.normalized * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
                animator.SetBool("walk", false);
            }
        }
    }

    private void MaintainGroundLevel()
    {
        if (rb.position.y > 0)
        {
            rb.position = new Vector3(rb.position.x, 0, rb.position.z);
        }
    }

    private IEnumerator DanceRoutine()
    {
        isDancing = true;
        isWalking = false;
        animator.SetBool("walk", false);
        animator.SetTrigger("dance");
        rb.velocity = Vector3.zero;

        if (danceMusicSource != null && danceMusicSource.clip != null)
        {
            danceMusicSource.Play();
        }

        float danceDuration = GetAnimationDuration("ShrekDance"); 
        if (danceDuration > 0)
        {
            yield return new WaitForSeconds(danceDuration);
        }
        else
        {
            Debug.LogWarning("Duração da animação de dança não encontrada ou inválida.");
            yield return new WaitForSeconds(5f); 
        }

      
        if (danceMusicSource != null && danceMusicSource.isPlaying)
        {
            danceMusicSource.Stop();
        }

        EndDance();
    }

    private void EndDance()
    {
        isDancing = false;
        animator.ResetTrigger("dance");
        animator.SetTrigger("idle");
    }

   
    private float GetAnimationDuration(string animationName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == animationName)
                {
                    return clip.length;
                }
            }
            Debug.LogWarning($"Animação '{animationName}' não encontrada.");
        }
        else
        {
            Debug.LogWarning("Animator ou RuntimeAnimatorController não configurado.");
        }
        return 0f;
    }

  
    private void SpawnDirtParticles()
    {
        if (dirtParticlesPrefab != null)
        {
            ParticleSystem dirtParticles = Instantiate(dirtParticlesPrefab, transform.position, Quaternion.identity);
            dirtParticles.Play();
            Destroy(dirtParticles.gameObject, dirtParticles.main.duration);
        }
    }
}