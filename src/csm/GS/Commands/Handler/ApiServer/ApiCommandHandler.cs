using System;
using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public abstract class ApiCommandHandler
    {
        public abstract Type GetDataType();

        public abstract void Parse(ApiCommandBase message);
    }

    public abstract class ApiCommandHandler<C> : ApiCommandHandler where C : ApiCommandBase
    {
        protected abstract void Handle(C command);

        public override Type GetDataType()
        {
            return typeof(C);
        }

        public override void Parse(ApiCommandBase command)
        {
            Handle((C) command);
        }
    }
}
