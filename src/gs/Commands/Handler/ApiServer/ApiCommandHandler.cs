using System;
using System.Net;
using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public abstract class ApiCommandHandler
    {
        public abstract Type GetDataType();

        public abstract void Parse(WorkerService worker, IPEndPoint sender, ApiCommandBase message);
    }

    public abstract class ApiCommandHandler<C> : ApiCommandHandler where C : ApiCommandBase
    {
        protected WorkerService worker;
        protected IPEndPoint sender;

        protected abstract void Handle(C command);

        public override Type GetDataType()
        {
            return typeof(C);
        }

        public override void Parse(WorkerService worker, IPEndPoint sender, ApiCommandBase command)
        {
            this.worker = worker;
            this.sender = sender;
            Handle((C) command);
        }
    }
}
