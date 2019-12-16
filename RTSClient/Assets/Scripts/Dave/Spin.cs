using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    private bool expand = true;
    void Update()
    {
        Vector3 rot;
        rot = transform.eulerAngles;
        rot.x += 1.5f;
        rot.z += 10;
        rot.y++;
        transform.eulerAngles = rot;

        Vector3 sca;
        sca = transform.localScale;
        if (expand)
        {
            sca.x = +0.3f;
            sca.y = +0.3f;
            sca.z = +0.3f;
            if (sca.x > 10)
            {
                expand = false;
            }
        }
        else
        {
            sca.x--;
            sca.y--;
            sca.z--;
            if (sca.x < 0)
            {
                expand = true;
            }
        }
        transform.localScale = sca;
    }
}
