using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotatableConnector : ConnectableComponent
{

    [SerializeField] Node node;

    protected override void Start()
    {
        base.Start();

        grabInteractable.activated.AddListener((ActivateEventArgs eventArgs) => GotPinched());
    }

    void GotPinched()
    {
        if (touchedNode != null)
        {
            // Flip node of the pipe which the elbow joint is to be connected to
            touchedNode.ParentFlipNodeToAcceptElbowJoint();
            touchedNode.isFlipped = true;

            // Flip the elbow joint's node so that the components connected next aren't inverted like the elbow joint is
            node.FlipElbowJointNode();
            node.isFlipped = true;
        }
    }

    protected override void OnTriggerExit(Collider col)
    {
        if (!isConnected && col.CompareTag("Node") && touchedNode != null)
        {
            if (col.gameObject == touchedNode.gameObject)
            {
                if (touchedNode.isFlipped)
                {
                    touchedNode.ParentFlipNodeToAcceptElbowJoint();
                    touchedNode.isFlipped = false;
                }
            }
        }

        if (!isConnected && node.isFlipped)
        {
            node.FlipElbowJointNode();
            node.isFlipped = false;
        }

        base.OnTriggerExit(col);
    }

}
