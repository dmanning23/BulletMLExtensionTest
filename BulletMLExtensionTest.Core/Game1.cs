using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BulletMLExtensionTest.Core
{
#if __IOS__ || ANDROID || WINDOWS_UAP
	public class Game1 : TouchGame
#else
	public class Game1 : MouseGame
#endif
	{
		public Game1()
		{
			IsMouseVisible = true;
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
#if !__IOS__
				this.Exit();
#endif
			}

			base.Update(gameTime);
		}

		public override IScreen[] GetMainMenuScreenStack()
		{
			return new IScreen[] { new MainScreen() };
		}
	}
}
