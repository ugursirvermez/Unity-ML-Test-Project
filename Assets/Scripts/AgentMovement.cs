using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

///<summary>
///Özetle; ML Agent ile birlikte Agent M'yi şöyle eğiteceğiz;
///1- İlk önce "Gözlem" yapacak. (CollectedObservations and DecisionRequester)
///2- "Karar" verecek.
///3- Kararına göre "Eylem"e geçecek. (ActionReceived)
///4- Eylemin sonucunda "Ödül" alacak. (OnTriggerEnter)
///</summary>

public class AgentMovement : Agent
{
    [SerializeField] private Transform target; //SecretFile objesi
    [SerializeField] private float speedforce=1f;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private TMP_Text Attempt;
    private int attempt=0;
    public override void OnEpisodeBegin(){
        transform.localPosition = SpawnPoint.localPosition; //İlk başladığında spawn noktasına dön
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition); //Sensöre gözlemleyeceği yeri söylüyorum.
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions){
        //Debug.Log(actions.DiscreteActions[0]);
        float x_axis = actions.ContinuousActions[0]; //X düzleminde gidecek.
        float z_axis = actions.ContinuousActions[1]; //Z düzelminde gidecek.

        transform.localPosition += new Vector3(x_axis,0,z_axis) * Time.deltaTime*speedforce;
    }
    public override void Heuristic(in ActionBuffers actionsOut){ //BehaviourType = Heuristic Only
        ActionSegment<float> continousAction = actionsOut.ContinuousActions; //Hareketleri oluştur.
        continousAction[0] = Input.GetAxisRaw("Horizontal"); 
        continousAction[1] = Input.GetAxisRaw("Vertical"); 

    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<SecretFile>(out SecretFile file)){//Hedefe ne kadar ulaştı?
        SetReward(1f); //Ödül
        Attempt.text="WIN";
        EndEpisode(); //Bir denemeyi bitiriyoruz
        }
        if(other.TryGetComponent<Walls>(out Walls wall)){ // Ne kadar duvara ulaştı?
        SetReward(-1f); //Duvara değdikçe ceza veriyoruz
        attempt++;
        Attempt.text ="Attempt: "+attempt;
        EndEpisode(); //Bir denemeyi bitiriyoruz
        }   
    }
}
