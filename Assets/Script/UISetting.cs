using UnityEngine;

/// <summary>
/// Gắn lên panel Setting: BtnSound (SFX/click), BtnMusic (BGM), BtnClose.
/// Kéo thả Image On/Off vào từng cặp; gán HomeUIController để đóng popup (gọi OnCloseSetting).
/// </summary>
public class UISetting : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private HomeUIController homeUI;

    [Header("BtnSound — SFX (click, bomb, lose, …)")]
    [SerializeField] private GameObject soundImageOn;
    [SerializeField] private GameObject soundImageOff;

    [Header("BtnMusic — nhạc nền")]
    [SerializeField] private GameObject musicImageOn;
    [SerializeField] private GameObject musicImageOff;

    private void OnEnable()
    {
        SyncFromSoundManager();
    }

    private void SyncFromSoundManager()
    {
        if (SoundManager.Instance == null)
            return;

        ApplySoundVisual(SoundManager.Instance.SfxEnabled);
        ApplyMusicVisual(SoundManager.Instance.BgmEnabled);
    }

    private static void SetPairActive(GameObject onObj, GameObject offObj, bool enabled)
    {
        if (onObj != null)
            onObj.SetActive(enabled);
        if (offObj != null)
            offObj.SetActive(!enabled);
    }

    private void ApplySoundVisual(bool sfxOn)
    {
        SetPairActive(soundImageOn, soundImageOff, sfxOn);
    }

    private void ApplyMusicVisual(bool musicOn)
    {
        SetPairActive(musicImageOn, musicImageOff, musicOn);
    }

    /// <summary>OnClick BtnSound trong Inspector.</summary>
    public void OnBtnSound()
    {
        if (SoundManager.Instance == null)
            return;

        bool next = !SoundManager.Instance.SfxEnabled;
        SoundManager.Instance.SetSfxEnabled(next);
        ApplySoundVisual(next);
        if (next)
            SoundManager.Instance.PlayUiClick();
    }

    /// <summary>OnClick BtnMusic trong Inspector.</summary>
    public void OnBtnMusic()
    {
        if (SoundManager.Instance == null)
            return;

        bool next = !SoundManager.Instance.BgmEnabled;
        SoundManager.Instance.SetBgmEnabled(next);
        ApplyMusicVisual(next);
        if (next)
            SoundManager.Instance.PlayUiClick();
    }

    /// <summary>OnClick BtnClose — đóng UI Setting (có kèm click + animation như HomeUIController).</summary>
    public void OnBtnClose()
    {
        if (homeUI != null)
            homeUI.OnCloseSetting();
    }
}
