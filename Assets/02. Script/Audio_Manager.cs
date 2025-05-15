using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public AudioSource BGM_audioSource;
    public AudioSource Effect_audioSource;
    public AudioSource Ment_audioSource;
    public AudioClip Bgm_Clip;

    // 버튼 0, 맞음 1, 틀림 2, 박수 3, 성공 4
    public AudioClip[] Effect_Clip;

    [Range(0f, 1f)]
    public float Current_BGM_Volume = 0.5f; // 기본값 설정

    public static Audio_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad 제거 - 씬 전환 시 재생성 허용
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 각 오디오 소스 초기화
        InitializeAudioSource(ref BGM_audioSource, 0);
        InitializeAudioSource(ref Effect_audioSource, 1);
        InitializeAudioSource(ref Ment_audioSource, 2);

        // BGM 설정 및 재생
        if (Bgm_Clip != null && BGM_audioSource != null)
        {
            BGM_audioSource.clip = Bgm_Clip;
            BGM_audioSource.loop = true;
            BGM_audioSource.volume = Current_BGM_Volume;
            BGM_audioSource.Play();
        }
    }

    // 오디오 소스 초기화 헬퍼 함수
    private void InitializeAudioSource(ref AudioSource source, int childIndex)
    {
        Transform child = transform.GetChild(childIndex);
        source = child.GetComponent<AudioSource>();

        if (source == null)
        {
            source = child.gameObject.AddComponent<AudioSource>();
        }
    }

    public void SetMute(bool isMute) // isMute이 true면 off, false면 on
    {
        if (isMute)
        {
            if (BGM_audioSource.volume > 0)
            {
                Current_BGM_Volume = BGM_audioSource.volume; // 현재 볼륨 저장
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
        Current_BGM_Volume = Mathf.Clamp01(volume); // 0~1 사이 값으로 제한
        BGM_audioSource.volume = Current_BGM_Volume;
    }

    /// <summary>
    /// 효과음 재생 함수
    /// 버튼 0, 맞음 1, 틀림 2, 박수 3, 성공 4
    /// </summary>
    public void PlayEffect(int index)
    {
        if (Effect_Clip != null && index >= 0 && index < Effect_Clip.Length && Effect_Clip[index] != null)
        {
            Effect_audioSource.PlayOneShot(Effect_Clip[index]);
        }
        else
        {
            Debug.LogWarning($"효과음 인덱스({index})가 범위를 벗어나거나, 클립이 없습니다.");
        }
    }

    /// <summary>
    /// 멘트 오디오 재생 함수
    /// </summary>
    public void PlayMent(AudioClip clip)
    {
        if (clip != null)
        {
            Ment_audioSource.Stop(); // 현재 재생 중인 사운드 정지
            Ment_audioSource.clip = clip;
            Ment_audioSource.Play();
        }
    }
}