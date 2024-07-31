using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace Test_PostJsonEvent
{
    public class EventService : MonoBehaviour
    {
        [SerializeField] private string serverURL;

        private const float cooldownBeforeSend = 3f;

        private CancellationTokenSource tokenSource = new();
        private HttpClient httpClient = new HttpClient();
        private EventListDTO chachedEvents = new();
        private float cooldownTimer;

        private void Start()
        {
            //TODO load chachedEvents from disk
            TrackEvent("dsds","sdsd");
        }

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                Task.Run(() => SendChachedEvents(tokenSource.Token));
            }
        }

        private void OnDestroy()
        {
            tokenSource.Cancel();
        }

        public void TrackEvent(string type, string data)
        {
            ChacheEvent(type, data);

            if (cooldownTimer <= 0)
            {
                cooldownTimer = cooldownBeforeSend;
                Task.Run(() => SendChachedEvents(tokenSource.Token));
            }
        }

        private async Task SendChachedEvents(CancellationToken token)
        {
            if (serverURL.Length == 0)
                return;
            if (chachedEvents.events.Count == 0)
                return;

            Debug.Log("Trying to send events to server");

            try
            {
                var content = CreateHttpContent();
                using HttpResponseMessage response = await httpClient.PostAsync(serverURL, content, token);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ClearChache();
                    Debug.Log("Events sucsessfully send to server");
                }
                else
                {
                    Debug.LogError($"Can't send events to server, status: {response.StatusCode}, {response.ReasonPhrase}");
                }

                cooldownTimer = 0;
            }
            catch (System.Exception exeption)
            {
                Debug.LogError($"Can't send events to server, message: {exeption.Message}");
            }
        }

        private void ChacheEvent(string type, string data)
        {
            EventDTO newEvent = new() { type = type, data = data };
            chachedEvents.events.Add(newEvent);
            //TODO save chachedEvents to disk
        }

        private void ClearChache()
        {
            chachedEvents.events.Clear();
            //TODO save chachedEvents to disk
        }

        private HttpContent CreateHttpContent()
        {
            string json = JsonConvert.SerializeObject(chachedEvents);
            HttpContent content = new StringContent(json);
            return content;
        }
    }
}