using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InputServer : MonoBehaviour
{
    private UserControl _control;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        _control = GetComponent<UserControl>();
        Serve();
    }

    private async Task Serve()
    {
        var server = new UdpClient(5678);
        while (true)
        {
            var data = (await server.ReceiveAsync()).Buffer;
            var msg = Encoding.UTF8.GetString(data, 0, data.Length);
            if (msg.StartsWith("angle: "))
            {
                var angle = float.Parse(msg[7..], CultureInfo.InvariantCulture);
                _control.Angle = angle / 2;
            }
            else if (msg.StartsWith("gas: "))
            {
                var rate = int.Parse(msg[5..]);
                if (rate == 0)
                {
                    print(rate);
                }
                _control.Engine =  (float)(rate / 100.0);
            }else if (msg.StartsWith("break: "))
            {
                var rate = int.Parse(msg[7..]);
                _control.Breaks = (float)(rate / 100.0);
            }
        }
    }
}
