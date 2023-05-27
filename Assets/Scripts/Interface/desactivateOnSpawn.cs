using Unity.Netcode;

public class desactivateOnSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false);
    }
}
