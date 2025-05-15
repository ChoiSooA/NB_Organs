using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public AudioSource BGM_audioSource;
    public AudioSource Effect_audioSource;
    public AudioSource Ment_audioSource;
    public AudioClip Bgm_Clip;

    // ��ư 0, ���� 1, Ʋ�� 2, �ڼ� 3, ���� 4
    public AudioClip[] Effect_Clip;

    [Range(0f, 1f)]
    public float Current_BGM_Volume = 0.5f; // �⺻�� ����

    public static Audio_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad ���� - �� ��ȯ �� ����� ���
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �� ����� �ҽ� �ʱ�ȭ
        InitializeAudioSource(ref BGM_audioSource, 0);
        InitializeAudioSource(ref Effect_audioSource, 1);
        InitializeAudioSource(ref Ment_audioSource, 2);

        // BGM ���� �� ���
        if (Bgm_Clip != null && BGM_audioSource != null)
        {
            BGM_audioSource.clip = Bgm_Clip;
            BGM_audioSource.loop = true;
            BGM_audioSource.volume = Current_BGM_Volume;
            BGM_audioSource.Play();
        }
    }

    // ����� �ҽ� �ʱ�ȭ ���� �Լ�
    private void InitializeAudioSource(ref AudioSource source, int childIndex)
    {
        Transform child = transform.GetChild(childIndex);
        source = child.GetComponent<AudioSource>();

        if (source == null)
        {
            source = child.gameObject.AddComponent<AudioSource>();
        }
    }

    public void SetMute(bool isMute) // isMute�� true�� off, false�� on
    {
        if (isMute)
        {
            if (BGM_audioSource.volume > 0)
            {
                Current_BGM_Volume = BGM_audioSource.volume; // ���� ���� ����
                BGM_audioSource.volume = 0;
            }
        }
        else
        {
            BGM_audioSource.volume = Current_BGM_Volume;
        }
    }

    public void SetBGMVolume(float volume)
    {
        Current_BGM_Volume = Mathf.Clamp01(volume); // 0~1 ���� ������ ����
        BGM_audioSource.volume = Current_BGM_Volume;
    }

    /// <summary>
    /// ȿ���� ��� �Լ�
    /// ��ư 0, ���� 1, Ʋ�� 2, �ڼ� 3, ���� 4
    /// </summary>
    public void PlayEffect(int index)
    {
        if (Effect_Clip != null && index >= 0 && index < Effect_Clip.Length && Effect_Clip[index] != null)
        {
            Effect_audioSource.PlayOneShot(Effect_Clip[index]);
        }
        else
        {
            Debug.LogWarning($"ȿ���� �ε���({index})�� ������ ����ų�, Ŭ���� �����ϴ�.");
        }
    }

    /// <summary>
    /// ��Ʈ ����� ��� �Լ�
    /// </summary>
    public void PlayMent(AudioClip clip)
    {
        if (clip != null)
        {
            Ment_audioSource.Stop(); // ���� ��� ���� ���� ����
            Ment_audioSource.clip = clip;
            Ment_audioSource.Play();
        }
    }
}