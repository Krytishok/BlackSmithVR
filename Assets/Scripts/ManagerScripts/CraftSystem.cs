using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using System.Collections;


public class CraftSystem : MonoBehaviour
{
    [SerializeField] GameObject Example;

    [SerializeField] GameObject[] _guards;

    [SerializeField] GameObject[] _handles;

    [SerializeField] XRSocketInteractor sellSocket;

    [SerializeField] Animator _coinAnimator;

    [SerializeField] GameObject _peasant;
    [SerializeField] Animator _peasantAnimator;
    [SerializeField] Transform _destination;
    [SerializeField] float time = 3f;


    public void sellSocketSelected()
    {
        GameObject sword = sellSocket.interactablesSelected[0].transform.gameObject;
        List<string> craftnames = sword.GetComponentInChildren<BlankTriggerManager>().GetListWithComponents();

        if(craftnames.Count > 0)
        {
            if(craftnames.Contains("Guard1") && craftnames.Contains("Handle1"))
            {
                Debug.Log("ПРЕДМЕТ ГОТОВ К ПРОДАЖЕ!");
                _coinAnimator.SetBool("Pay", true);
            }
        }

    }


    public void CoinTaken()
    {
        Example.SetActive(false);

        Destroy(sellSocket.interactablesSelected[0].transform.gameObject);

        

    }

    IEnumerator MoveAndRotateTarget(GameObject unit, Vector3 destination, float time)
    {
        
        // Получаем Transform один раз для оптимизации
        Transform unitTransform = unit.transform;

        Vector3 startPosition = unitTransform.position;
        Quaternion startRotation = unitTransform.rotation;

        // Вычисляем направление взгляда
        Vector3 direction = (destination - startPosition).normalized;
        Quaternion finalRotation = startRotation;

        if (direction != Vector3.zero)
        {
            finalRotation = Quaternion.LookRotation(direction);
        }

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Перемещаем и вращаем конкретный объект _peasant
            unitTransform.position = Vector3.Lerp(startPosition, destination, smoothT);
            unitTransform.rotation = Quaternion.Slerp(startRotation, finalRotation, smoothT);

            elapsedTime += Time.deltaTime;

            Debug.Log($"Moving: {elapsedTime}");
            yield return null;
        }

        unitTransform.position = destination;
        unitTransform.rotation = finalRotation;
    }



}


