﻿//Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
//Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework;
using osu.Framework.Configuration;
using osu.Framework.GameModes;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Background;
using osu.Game.Graphics.Containers;

namespace osu.Game.GameModes
{
    public abstract class OsuGameMode : GameMode
    {
        internal BackgroundMode Background { get; private set; }

        /// <summary>
        /// Override to create a BackgroundMode for the current GameMode.
        /// Note that the instance created may not be the used instance if it matches the BackgroundMode equality clause.
        /// </summary>
        protected virtual BackgroundMode CreateBackground() => null;

        internal virtual bool ShowToolbar => true;

        protected new OsuGame Game => base.Game as OsuGame;

        protected float ToolbarPadding => ShowToolbar ? Game.Toolbar.DrawHeight : 0;

        private bool boundToBeatmap;
        private Bindable<WorkingBeatmap> beatmap;

        public WorkingBeatmap Beatmap
        {
            get
            {
                bindBeatmap();
                return beatmap.Value;
            }
            set
            {
                bindBeatmap();
                beatmap.Value = value;
            }
        }

        private void bindBeatmap()
        {
            if (beatmap == null)
                beatmap = new Bindable<WorkingBeatmap>();

            if (!boundToBeatmap)
            {
                beatmap.ValueChanged += beatmap_ValueChanged;
                boundToBeatmap = true;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (boundToBeatmap)
                beatmap.ValueChanged -= beatmap_ValueChanged;

            base.Dispose(isDisposing);
        }

        private void beatmap_ValueChanged(object sender, EventArgs e)
        {
            OnBeatmapChanged(beatmap.Value);
        }

        protected override void Load(BaseGame game)
        {
            base.Load(game);
            beatmap = (game as OsuGameBase)?.Beatmap;
        }

        public override bool Push(GameMode mode)
        {
            OsuGameMode nextOsu = mode as OsuGameMode;
            if (nextOsu != null)
            {
                nextOsu.beatmap = beatmap;
            }

            return base.Push(mode);
        }

        protected virtual void OnBeatmapChanged(WorkingBeatmap beatmap)
        {

        }

        protected override void OnEntering(GameMode last)
        {
            OsuGameMode lastOsu = last as OsuGameMode;

            BackgroundMode bg = CreateBackground();

            if (lastOsu?.Background != null)
            {
                if (bg == null || lastOsu.Background.Equals(bg))
                    //we can keep the previous mode's background.
                    Background = lastOsu.Background;
                else
                {
                    lastOsu.Background.Push(Background = bg);
                }
            }
            else if (bg != null)
            {
                AddInternal(new ParallaxContainer
                {
                    Depth = float.MinValue,
                    Children = new[]
                    {
                        Background = bg
                    }
                });
            }

            base.OnEntering(last);
        }

        protected override bool OnExiting(GameMode next)
        {
            OsuGameMode nextOsu = next as OsuGameMode;

            if (Background != null && !Background.Equals(nextOsu?.Background))
            {
                if (nextOsu != null)
                    //We need to use MakeCurrent in case we are jumping up multiple game modes.
                    nextOsu.Background.MakeCurrent();
                else
                    Background.Exit();
            }

            return base.OnExiting(next);
        }
    }
}
