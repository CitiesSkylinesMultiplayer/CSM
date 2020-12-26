using ColossalFramework.UI;
using CSM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CSM.Panels
{
    class JoinRequestPanel : UIPanel
    {
        private UIButton acceptButton;
        private UIButton rejectButton;

        public override void Start()
        {
            //base setup of the panel
            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);
            UIView view = UIView.GetAView();

            //Center window
            relativePosition = new Vector3(view.fixedWidth / 2f - 150f, view.fixedHeight / 2f - 150f);
            width = 300;
            height = 300;

            //Title Label
            //UILabel Title = this.CreateTitleLabel("")

            base.Start();
        }
    }
}
