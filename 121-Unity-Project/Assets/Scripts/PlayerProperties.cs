using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerProperties : NetworkBehaviour
{
    [SyncVar]
    public string username;
}
