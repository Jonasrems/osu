﻿using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Configuration;

namespace osu.Game.Overlays.Options.Graphics
{
    public class LayoutOptions : OptionsSubsection
    {
        protected override string Header => "Layout";

        private CheckBoxOption fullscreen, letterboxing;

        public LayoutOptions()
        {
            Children = new Drawable[]
            {
                new SpriteText { Text = "Resolution: TODO dropdown" },
                fullscreen = new CheckBoxOption { LabelText = "Fullscreen mode" },
                letterboxing = new CheckBoxOption { LabelText = "Letterboxing" },
                new SpriteText { Text = "Horizontal position" },
                new SpriteText { Text = "TODO: slider" },
                new SpriteText { Text = "Vertical position" },
                new SpriteText { Text = "TODO: slider" },
            };
        }

        protected override void Load(BaseGame game)
        {
            base.Load(game);
            var osuGame = game as OsuGameBase;
            if (osuGame != null)
            {
                fullscreen.Bindable = osuGame.Config.GetBindable<bool>(OsuConfig.Fullscreen);
                letterboxing.Bindable = osuGame.Config.GetBindable<bool>(OsuConfig.Letterboxing);
            }
        }
    }
}