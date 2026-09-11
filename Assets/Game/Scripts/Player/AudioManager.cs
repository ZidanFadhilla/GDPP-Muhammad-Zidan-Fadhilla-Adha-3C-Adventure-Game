using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _footstepSfx;
    [SerializeField]
    private AudioSource _attackSfx;
    [SerializeField]
    private AudioSource _glideSfx;
    [SerializeField]
    private AudioSource _landingSfx;


    private void PlayFootstepSfx() {
        _footstepSfx.volume = Random.Range(0.7f, 1f);
        _footstepSfx.pitch = Random.Range(0.5f, 2.5f);
        _footstepSfx.Play();
    }

    private void PlayAttackSfx() {
        _attackSfx.volume = Random.Range(0.7f, 1f);
        _attackSfx.pitch = Random.Range(0.5f, 2.5f);
        _attackSfx.Play();
    }

    private void PlayLandingSfx() {
        _attackSfx.volume = Random.Range(0.7f, 1f);
        _attackSfx.pitch = Random.Range(0.5f, 2.5f);
        _landingSfx.Play();
    }

    public void PlayGlideSfx() {
        _glideSfx.Play();
    }

    public void StopGlideSfx() {
        _glideSfx.Stop();
    }
}
