using UnityEngine;
using System.Collections;

public class ProtoBufDemo : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        LoginRequest();
        LoginResponse();
    }



    private void LoginRequest()
    {

        // client side code
        var msg = ClientMessage.CreateBuilder().LoginUserRequest.ToBuilder()
                                    .SetUserName("test user")
                                    .SetHashedPassword("123456789")
                                    .Build();

        ClientMessage builder = ClientMessage.CreateBuilder()
            .SetLoginUserRequest(msg)
            .SetType(ClientMessageType.LOGIN_USER_REQUEST)
            .Build();

        // now login msg is ready to send over network to server (seralize)
        var loginBytes = builder.ToByteArray();

        // deseralize
        var loginObject = ClientMessage.ParseFrom(loginBytes);

        Debug.Log(loginObject.Type);
        Debug.Log(loginObject.LoginUserRequest.UserName);
    }


    private void LoginResponse()
    {

        var msg = ServerMessage.CreateBuilder().LoginUserResponse.ToBuilder()
            .SetMetadataVersion(1)
            .SetGameVersion(1)
            .Build();

        ServerMessage loginResponseBuilder = ServerMessage.CreateBuilder()
           .SetLoginUserResponse(msg)
           .SetType(ServerMessageType.LOGIN_USER_RESPONSE)
           .Build();



        // seralize
        var loginRes = loginResponseBuilder.ToByteArray();

        // de-seralize
        var serverMsgReceived = ServerMessage.ParseFrom(loginRes);

        Debug.Log(serverMsgReceived.Type);
        Debug.Log(serverMsgReceived.LoginUserResponse.GameVersion);
    }

}
