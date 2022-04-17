
using Hkmp.Api.Client;
using Hkmp.Api.Server;

namespace Satchel.HkmpPipe{
    public class HkmpPipe{
        public string mod;
        public bool isServer;
        public event EventHandler<RecievedEventArgs> OnRecieve;
        public HkmpPipe(string mod,bool isServer){
            this.mod = mod;
            this.isServer = isServer;
        }

        public void startListening(){
            if(isServer){
                // register for server events
                Server.Instance.OnRecieve += (_,R) => handleRecieve(R.packet);
            } else {
                // register for client events
                Client.Instance.OnRecieve += (_,R) => handleRecieve(R.packet);
            }
        }

        internal void handleRecieve(GenericPacket p){
            if(p.mod != this.mod) { return; } // only recieve your own mods events
            OnRecieve?.Invoke(this,new RecievedEventArgs{
                packet = p
            });
        }

        public void SendToAll(ushort fromPlayer,string eventName,string eventData,bool reliable = true,bool sameScene = false){
            this.Send(fromPlayer,0,eventName,eventData,true,true,reliable,sameScene);
        }

        public void Send(ushort fromPlayer,ushort toPlayer,string eventName,string eventData,bool rebroadcast = true,bool broadcastToAll = false,bool reliable = true,bool sameScene = false){
            if(isServer){
                if(broadcastToAll){
                    Server.Instance.sendToAll(fromPlayer,this.mod,eventName,eventData,reliable,sameScene);
                } else {
                    Server.Instance.send(fromPlayer,toPlayer,this.mod,eventName,eventData,rebroadcast,broadcastToAll,reliable);
                }
            } else {
                Client.Instance.send(fromPlayer,toPlayer,this.mod,eventName,eventData,rebroadcast,broadcastToAll,reliable,sameScene);
            }
        }
    }
}