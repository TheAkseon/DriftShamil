using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class DriftManager : MonoBehaviour
{
    public Rigidbody CarRigidbody;
    public TMP_Text TotalScoreCountText;
    public TMP_Text currentScoreText;
    public TMP_Text MultiplyText;

    private float speed = 0;
    private float driftAngle = 0;
    private float driftMultiply = 1;
    private float currentScore;
    private float totalScore;

    private bool isDrifting = false;

    public float minimumSpeed = 5;
    public float minimumAngle = 10;
    public float driftingDelay = 0.2f;
    public GameObject driftingObject;

    private IEnumerator stopDriftingCoroutine = null;

    private void Start()
    {
        driftingObject.SetActive(false);
    }

    private void Update()
    {
        ManageDrift();
        ManageUI();
    }
    private void ManageDrift()
    {
        speed = CarRigidbody.velocity.magnitude;
        driftAngle = Vector3.Angle(CarRigidbody.transform.forward, (CarRigidbody.velocity + CarRigidbody.transform.forward).normalized);
        if (driftAngle > 120)
        {
            driftAngle = 0;
        }
        if (driftAngle >= minimumAngle && speed > minimumSpeed)
        {
            if (!isDrifting || stopDriftingCoroutine != null)
            {
                StartDrift();
            }
        }
        else
        {
            if (isDrifting && stopDriftingCoroutine == null)
            {
                StopDrift();
            }
        }
        if (isDrifting)
        {
            currentScore += Time.deltaTime * driftAngle * driftMultiply;
            driftMultiply += Time.deltaTime;
            driftingObject.SetActive(true);
        }
    }

    async void StartDrift()
    {
        if (!isDrifting)
        {
            await Task.Delay(Mathf.RoundToInt(1000 * driftingDelay));
            driftMultiply = 1;
        }
        if (stopDriftingCoroutine != null)
        {
            StopCoroutine(stopDriftingCoroutine);
            stopDriftingCoroutine = null;
        }
        isDrifting = true;
    }
    private void StopDrift()
    {
        stopDriftingCoroutine = StoppingDrift();
        StartCoroutine(stopDriftingCoroutine);
    }
    private IEnumerator StoppingDrift()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(driftingDelay * 4f);
        totalScore += currentScore;
        isDrifting = false;
        yield return new WaitForSeconds(0.5f);
        currentScore = 0;
        driftingObject.SetActive(false);
    }

    private void ManageUI()
    {
        TotalScoreCountText.text = totalScore.ToString("###,###,000");
        MultiplyText.text = driftMultiply.ToString("###,###,##0.0") + "X";
        currentScoreText.text = currentScore.ToString("###,###,000");
    }
}
