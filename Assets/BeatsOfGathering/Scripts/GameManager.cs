using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{

    //Singleton
    public static GameManager Instance;


    //sfx //
    public AudioSource theMusic;
    [SerializeField] public AudioClip crowdCheer;
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioClip goodHitSound;
    [SerializeField] private AudioClip perfectHitSound;
    [SerializeField] private AudioClip missHitSound;
    [SerializeField] private AudioClip wowSound;
    [SerializeField] private ParticleSystem perfectHitParticles;
    [SerializeField] private ParticleSystem celebrationParticles;
    public bool startPlaying;
    public BeatScroller beatScroller;

    //Score variables
    public int currentScore;
    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;
    public Text scoreText;
    public Text multiplierText;


    [SerializeField] private TextMeshProUGUI comboText;
    public int combo;

    //Celebration Meter
    // Celebration meter
    [Header("Celebration Meter")]
    public CelebrationMeter celebrationMeter;   // drag your meter in the Inspector
    [Range(0f, 1f)] public float addOnNormal = 0.010f;
    [Range(0f, 1f)] public float addOnGood = 0.020f;
    [Range(0f, 1f)] public float addOnPerfect = 0.035f;
    [Range(-1f, 0f)] public float addOnMiss = -0.025f;
    public bool celebrationReached;



    //Results Screen Stats(Reset to private later)
    public float totalNotes;
    public float normalHits;
    public float GoodHits;
    public float PerfectHits;
    public float MissedHits;

    public GameObject resultsScreen;
    public Text percentHitText;
    public Text normalHitText;
    public Text goodHitText;
    public Text perfectHitText;
    public Text missedHitText;
    public Text rankText;
    public Text finalScoreText;


    private void Awake()
    {
        comboText.gameObject.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        celebrationParticles.gameObject.SetActive(false);
    }

    private void Start()
    {
        currentMultiplier = 1;
        currentScore = 0;
        scoreText.text = "Score: " + currentScore;

        totalNotes = FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
    }


    //Manage score and ranking
    private void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                beatScroller.hasStarted = true;
                theMusic.Play();
            }
        }
        else
        {
            if (celebrationReached)
            {
                //Instantiate(perfectHitParticles, transform.position, perfectHitParticles.transform.rotation);      
                //celebrationParticles.Play();  
            }

            if (!theMusic.isPlaying && !resultsScreen.activeInHierarchy)
            {
                resultsScreen.SetActive(true);
                normalHitText.text = "Normal Hits: " + normalHits;
                goodHitText.text = GoodHits.ToString();
                perfectHitText.text = PerfectHits.ToString();
                missedHitText.text = "" + MissedHits;

                float totalHit = normalHits + GoodHits + PerfectHits;
                float percentHit = (totalHit / totalNotes) * 100f;
                percentHitText.text = percentHit.ToString("F1") + "%";

                string rankVal = "F";

                if (percentHit > 40)
                {
                    rankVal = "D";
                    if (percentHit > 55)
                    {
                        rankVal = "C";
                        if (percentHit > 70)
                        {
                            rankVal = "B";
                            if (percentHit > 85)
                            {
                                rankVal = "A";
                                if (percentHit > 95)
                                {
                                    rankVal = "S";
                                }
                            }
                        }
                    }
                }

                rankText.text = rankVal;
                finalScoreText.text = currentScore.ToString();

            }
        }
    }


    //Score per hit precision methods
    public void NoteHit()
    {
        
        Debug.Log("Hit on time");
        multiplierTracker++;

        if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
        {
            multiplierTracker = 0;

            if (currentMultiplier < multiplierThresholds.Length)
            {
                currentMultiplier++;
            }
        }
        multiplierText.text = "Multiplier: x" + currentMultiplier;
        //currentScore += scorePerNote * currentMultiplier;
        scoreText.text = "Score: " + currentScore;


        // Play crowd cheer sound
        if (celebrationReached)
        {
            celebrationParticles.gameObject.SetActive(true);
            celebrationParticles.Play();
            theMusic.PlayOneShot(crowdCheer, 1f);
        }

        if(comboText.gameObject.activeInHierarchy == false)
        {
            comboText.gameObject.SetActive(true);
        }
        combo++;
        UpdateComboText();
    }

    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        normalHits++;
        theMusic.PlayOneShot(normalHitSound, 0.7f);
        if (celebrationMeter) celebrationMeter.Add(addOnNormal);   // <—

        NoteHit();
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        GoodHits++;
        theMusic.PlayOneShot(goodHitSound, 0.7f);
        if (celebrationMeter) celebrationMeter.Add(addOnGood);     // <—

        NoteHit();
    }

    //I made the celebration effect in perfect, please later move the logic separately when the celebration logig is ready
    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        PerfectHits++;
        Instantiate(perfectHitParticles, transform.position, perfectHitParticles.transform.rotation);
        theMusic.PlayOneShot(perfectHitSound, 0.7f);
        if (celebrationMeter) celebrationMeter.Add(addOnPerfect);  // <—


        theMusic.PlayOneShot(crowdCheer, 0.5f);
        theMusic.PlayOneShot(wowSound, 1f);
        NoteHit();
    }

    public void NoteMissed()
    {
        Debug.Log("Missed Note");
    
        combo = 0;
        UpdateComboText();
        comboText.gameObject.SetActive(false);

        MissedHits++;
        currentMultiplier = 1;
        multiplierTracker = 0;
        theMusic.PlayOneShot(missHitSound, 0.7f);

        //Reset celebration meter on miss
        if (celebrationMeter) celebrationMeter.Add(addOnMiss); // <—

        celebrationParticles.Stop();
        multiplierText.text = "Multiplier: x" + currentMultiplier;

    }

    private void UpdateComboText()
    {
        comboText.text = combo.ToString();
    }

}
