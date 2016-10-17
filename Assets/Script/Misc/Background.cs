using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Background : MonoBehaviour {
    Image bg;
	// Use this for initialization
	void Start () {
        bg = this.GetComponent<Image>();
        StartCoroutine(changeColor());
	}

    IEnumerator changeColor() {
        while (true) {
            Color zh = new Color(bg.color.r + Random.Range(-0.1f, 0.1f),
                                bg.color.g + Random.Range(-0.1f, 0.1f),
                                bg.color.b + Random.Range(-0.1f, 0.1f));

            float deltaRed, deltaGreen, deltaBlue;
            deltaRed = bg.color.r - zh.r;
            deltaGreen = bg.color.g - zh.g;
            deltaBlue = bg.color.b - zh.b;

            float time = 5.0f;
            int divs = 20;

            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(time / divs);
                bg.color += new Color(deltaRed / divs, deltaGreen / divs, deltaBlue / divs);
            }
        }
    }
}
