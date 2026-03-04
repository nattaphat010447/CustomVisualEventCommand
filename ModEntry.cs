using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomVisualEventCommand
{
    public class ModEntry : Mod
    {
        private bool isShowingAnim = false;
        private Texture2D? currentTexture; 

        private int totalFrames = 1;
        private int currentFrame = 0;
        private int frameDelay = 100;
        private double timeElapsed = 0; 

        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Custom Visual Event Command is loaded and ready!", LogLevel.Info);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;

            helper.ConsoleCommands.Add("test_anim", "Test the ShowAnim command with a sample event script.", this.Command_TestAnim);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Event.RegisterCommand("ShowAnim", this.Command_ShowAnim);
            Event.RegisterCommand("StopAnim", this.Command_StopAnim);
        }

        private void Command_ShowAnim(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (args.Length < 2)
                {
                    this.Monitor.Log($"[ShowAnim] Invalid arguments. Usage: ShowAnim <imageName> [totalFrames] [frameDelay]", LogLevel.Warn);
                    @event.CurrentCommand++;
                    return;
                }
                string imageName = args[1]; 

                this.totalFrames = args.Length > 2 ? int.Parse(args[2].Trim()) : 1; 
                this.frameDelay = args.Length > 3 ? int.Parse(args[3].Trim()) : 100;

                try
                {
                    this.currentTexture = Game1.content.Load<Texture2D>(imageName);
                }
                catch
                {
                    this.currentTexture = this.Helper.ModContent.Load<Texture2D>($"assets/{imageName}.png");
                }

                this.currentFrame = 0;
                this.timeElapsed = 0;
                this.isShowingAnim = true;

            }
            catch (Exception ex)
            {
                this.Monitor.Log($"[ShowAnim] Error occurred while loading animation: {ex.Message}", LogLevel.Error);
            }

            @event.CurrentCommand++; 
        }

        private void Command_StopAnim(Event @event, string[] args, EventContext context)
        {
            this.isShowingAnim = false;
            @event.CurrentCommand++;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!this.isShowingAnim || this.totalFrames <= 1) return;

            this.timeElapsed += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.timeElapsed >= this.frameDelay)
            {
                this.currentFrame++;
                if (this.currentFrame >= this.totalFrames)
                {
                    this.currentFrame = 0; 

                }
                this.timeElapsed = 0; 

            }
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (!this.isShowingAnim || this.currentTexture == null) return;

            int frameWidth = this.currentTexture.Width / this.totalFrames;
            int frameHeight = this.currentTexture.Height;

            Rectangle sourceRect = new Rectangle(this.currentFrame * frameWidth, 0, frameWidth, frameHeight);

            float screenWidth = Game1.uiViewport.Width;
            float screenHeight = Game1.uiViewport.Height;

            float scaleX = screenWidth / frameWidth;
            float scaleY = screenHeight / frameHeight;
            float finalScale = Math.Min(scaleX, scaleY); 

            Vector2 position = new Vector2(
                (screenWidth - (frameWidth * finalScale)) / 2f,
                (screenHeight - (frameHeight * finalScale)) / 2f
            );

            e.SpriteBatch.Draw(
                texture: this.currentTexture,
                position: position,
                sourceRectangle: sourceRect,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: finalScale,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );
        }

        private void Command_TestAnim(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log($"[TestAnim] Must load a save and enter the game world before running tests!", LogLevel.Warn);
                return;
            }

            string imageName = args.Length > 0 ? args[0] : "test_image";
            string frames = args.Length > 1 ? args[1] : "1";
            string delay = args.Length > 2 ? args[2] : "100";

            string eventScript = $"none/1 1/farmer 1 1 2/ShowAnim {imageName} {frames} {delay}/pause 5000/StopAnim/end";

            Game1.currentLocation.startEvent(new Event(eventScript));

            this.Monitor.Log($"[TestAnim] Starting sample event script: {eventScript}", LogLevel.Info);
        }
    }
}