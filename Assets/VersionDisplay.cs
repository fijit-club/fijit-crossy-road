using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionDisplay : MonoBehaviour
{
    private void Awake()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = Application.version;
    }
}
