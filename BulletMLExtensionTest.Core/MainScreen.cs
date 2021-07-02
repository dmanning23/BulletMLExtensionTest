using BulletMLLib;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using System;
using System.Threading.Tasks;

namespace BulletMLExtensionTest.Core
{
	public class MainScreen : WidgetScreen
	{
		#region Properties

		Texture2D BulletTexture { get; set; }

		Myship myship;
		Mover mover;

		MoverManager _moverManager;

		private BulletPattern pattern;

		Slider BulletSpeedSlider;
		Slider LifeSlider;
		Slider RepeatRateSlider;
		Slider ClipSizeSlider;

		#endregion //Properties

		#region Methods

		public MainScreen() : base("MainScreen")
		{
			CoverOtherScreens = true;
			CoveredByOtherScreens = false;
		}

		public override async Task LoadContent()
		{
			await base.LoadContent();

			myship = new Myship();

			_moverManager = new MoverManager(myship.Position);

			BulletTexture = Content.Load<Texture2D>("bullet");

			try
			{
				//load the pattern
				pattern = new BulletPattern(_moverManager);
				pattern.ParseXML("fire_slowshot", Content);
			}
			catch (Exception ex)
			{
				await ScreenManager.ErrorScreen(ex);
			}

			var stack = new StackLayout(StackAlignment.Top)
			{
				Position = new Point(Resolution.TitleSafeArea.Right, Resolution.TitleSafeArea.Top),
				Horizontal = HorizontalAlignment.Right,
				Vertical = VerticalAlignment.Top
			};

			//Add the scrollbars
			BulletSpeedSlider = AddSlider(stack, 0.1f, 20f, _moverManager.BulletSpeed, "Speed");
			LifeSlider = AddSlider(stack, 1f, 100f, _moverManager.Life, "Life");
			RepeatRateSlider = AddSlider(stack, 0.1f, 20f, _moverManager.RepeatRate, "Repeat Rate");
			ClipSizeSlider = AddSlider(stack, 1f, 100f, _moverManager.ClipSize, "Clip Size");

			BulletSpeedSlider.OnDrag += ((obj, e) => {
				_moverManager.BulletSpeed = BulletSpeedSlider.SliderPosition;
			});
			LifeSlider.OnDrag += ((obj, e) => {
				_moverManager.Life = LifeSlider.SliderPosition;
			});
			RepeatRateSlider.OnDrag += ((obj, e) => {
				_moverManager.RepeatRate = RepeatRateSlider.SliderPosition;
			});
			ClipSizeSlider.OnDrag += ((obj, e) => {
				_moverManager.ClipSize = ClipSizeSlider.SliderPosition;
			});

			AddItem(stack);

			var buttonLabel = new Label("Fire", Content, FontSize.Medium)
			{
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Center,
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopBottom),
			};
			var button = new RelativeLayoutButton()
			{
				Position = new Point(Resolution.TitleSafeArea.Center.X, Resolution.TitleSafeArea.Bottom),
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopBottom),
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Bottom,
				Size = new Vector2(300, 100),
				HasOutline = true,
			};
			button.Layout.AddItem(buttonLabel);
			AddItem(button);
			button.OnClick += Button_OnClick;

			myship.Init();
		}

		private void Button_OnClick(object sender, InputHelper.ClickEventArgs e)
		{
			AddBullet();
		}

		private Slider AddSlider(StackLayout stack, float min, float max, float start, string sliderName)
		{
			//add the title
			stack.AddItem(new Label(sliderName, Content, FontSize.Small)
			{
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopRight)
			});

			//add the label
			var label = new Label("", Content, FontSize.Small)
			{
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopRight)
			};
			stack.AddItem(label);

			//add the slider
			var slider = new Slider()
			{
				Min = min,
				Max = max,
				SliderPosition = start,
				HandleSize = new Vector2(24, 32),
				Size = new Vector2(256, 32),
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopRight)
			};
			stack.AddItem(slider);

			label.Text = slider.SliderPosition.ToString();

			slider.OnDrag += ((obj, e) => {
				label.Text = slider.SliderPosition.ToString();
			});

			return slider;
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			_moverManager.Update();

			myship.Update();
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			ScreenManager.SpriteBatchBegin();

			foreach (Mover mover in _moverManager.movers)
			{
				ScreenManager.SpriteBatch.Draw(BulletTexture, mover.pos, Color.Red);
			}

			ScreenManager.SpriteBatch.Draw(BulletTexture, myship.pos, Color.Green);

			ScreenManager.SpriteBatchEnd();
		}

		private void AddBullet()
		{
			//clear out all the bulelts
			_moverManager.Clear();

			//add a new bullet in the center of the screen
			mover = (Mover)_moverManager.CreateTopBullet();
			mover.pos = Resolution.TitleSafeArea.Center.ToVector2();
			mover.InitTopNode(pattern.RootNode);
		}

		#endregion //Methods
	}
}
