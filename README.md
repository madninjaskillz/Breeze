
# Breeze
## Collection of services for monogame mostly focused on MVVM responsive UI
Breeze is a collection of helper services designed to make working with monogame simpler and easier. It includes:

* Breeze.UI - An MVVM UI framework heavily influenced by WinRT XAML
* Breeze.Storage - An abstracted storage provider allowing one interface to do everything you need on any platform no matter whether you are in app storage, local storage or PAK style folder blobs
* Breeze.Font - A wrapper around sprite fonts that allows auto selection of font based on render size.
* Breeze.AssetLibrary - Empowers you to stream assets on demand with intelligent caching
* Breeze.Benchmark - Benchmark code simply within a using statements scope with on screen charts when required.
* Breeze.Debug - A replacement to Debug.WriteLine that allows writing to in-game UI.
* Breeze.SpriteBatch - A wrapped SpriteBatch with many helper methods to make some basic tasks basic.
* Breeze.Input - Event driven Input service supporting KB+Mouse/TouchScreen and gamepad with ability to remap controls, ignore inputs when screen is not focused and supporting advanced combinations such as modifier keys and mouse buttons, button holds and double clicks.

#### Background
Breeze started life as a framework for my own personal project but as it grew more competent I realised it needed to be separated out into its own project. Early versions of this may make this very apparent with references to the previous project and maybe the odd hardcoded value.

As such documentation is currently behind the curve and the following is a very brief overview.

#### Using Breeze
Currently breeze is not modular as the main part, the main UI element requires most of the other parts to function properly.
Once you have added breeze to your project, you will need to modify you main Game.CS:

Firstly, you need an instance variable to use Breeze of type BreezeInstance. For this example we will assume (for simplicity sakes we have a public static in Game.CS)

    public static BreezeInstance Breeze;

In LoadContent() you will need to init Breeze:

    var thisspriteBatch = new SpriteBatch(graphics.GraphicsDevice);
    Solids.Breeze = new BreezeInstance(Content, thisspriteBatch, this);

You should also attach the keyboard event:
    
    #if !ANDROID
                Window.TextInput += Solids.Breeze.TextInputHandler;
    #endif

At the top of your Update():

    Solids.Breeze.Update(gameTime);

And finally, in your Draw(), after the graphicsDevice.Clear but before any spritebatch shenanigans:

    Solids.Breeze.Draw(gameTime, Solids.ShowDebug);

  You are now up and running.
    
### Using Breeze.UI

#### Creating a Screen

I would recommend creating a "screens" folder in the games root folder, and another folder for each screen.

In each of those subfolders we would expect to see three files:
*	{SCREENNAME}Screen.cs
*	{SCREENNAME}ViewModel.cs
*	{SCREENNAME}Screen.xml

If you have used WinRT XAML this should all be fairly natural.

#### Code Behind
The Screen.cs should inherit from DataboundScreen. This is similar to the 'code behind' of XAML. You will need to include the following:

  

    public override async Task Initialise()
            {
		DataContext = new LogInSignInViewModel();
		LogInSignInViewModel vm = (LogInSignInViewModel)DataContext;
		await base.Initialise();
		LoadXAML();
		UpdateAllAssets();
		Update(new GameTime());
    		SetBindings();
	    }

Hopefully, this boilerplate will get a little more sane soon.

#### ViewModel

The ViewModel.cs should inherit from MGUIViewModel.

Bindable variables take a very similar pattern to a common WinRT pattern:


    private bool forgotEnabled;
    public bool ForgotEnabled { get => forgotEnabled; set => Set(ref forgotEnabled, value); }


#### View

UI is based on a screen of a screen from 0 to 1 in x+y directions. However, when a child of an element, 0,0 position is its parents position but width and height is still the same scale. (options will be added to make width and height optionally scaled to parents size)

While it can be defined with code in the code behind, it is recommended to use the XML to construct the view. Here is an example:

        <Screen FixedAspectRatio = "0.8">
      <Resources>
        <Template Name="StandardButtonTemplate">
          <ButtonAsset Position="0,0,0.75,0.08">
            <RectangleAsset Position="0,0,0.75,0.08" Blur="8" BackgroundColor="Color.Black*0.9">
              <ContentAsset>
              </ContentAsset>
            </RectangleAsset>
    
            <RectangleAsset Position="0,0,0.75,0.08" Blur="8" BackgroundColor="Color.Green*0.6">
              <ContentAsset>
              </ContentAsset>
            </RectangleAsset>
          </ButtonAsset>
        </Template>
      </Resources>
      <BoxShadowAsset Color="Color.White" Position="0.15,0.15,0.75,0.75">
        <RectangleAsset BorderColor="0xFF99DBFF" BrushSize="0" Position="0,0, 0.75, 0.75" BackgroundColor="Color.White * 0.65" Blur="18">
          <StackAsset Position="0,0,0.75,0.75">
            <RectangleAsset BorderColor="Color.White" BrushSize="0" Position="0,0, 0.75, 0.05" BackgroundColor="Color.Purple * 0.65" Blur="8">
              <FontAsset Text="Log in to ezmuze central" FontColor="Color.White" Position="0,0,0.75,0.05" FontFamily="EuroStile" Margin="0,0,0,0.025"/>
            </RectangleAsset>
            <FontAsset Text="Please enter your user details for ezmuze central
    to enable full functionality." FontColor="Color.Black" Position="0,0,0.5,0.03" FontFamily="Segoe" Margin="0.025,0,0,0.02" MultiLine="true" AntiAlias="false" />
            <FontAsset Text="Username" FontColor="Color.Black*0.9" Position="0, 0, 0.7, 0.03" FontFamily="SegoeLight" Margin="0.025,0,0,0.02" />
            <TextboxAsset Position="0,0,0.7,0.048" Text="BoundTo=UserName,bindingdirection=TwoWay" FontMargin="0.003" XName="UserNameButton" Margin="0,0,0.025,0.02" />
    
            <FontAsset Text="Password" FontColor="Color.Black*0.9" Position="0, 0, 0.7, 0.03" FontFamily="SegoeLight" Margin="0.025,0,0,0.02" />
            <TextboxAsset Position="0,0,0.7,0.048" Text="BoundTo=PasswordText,bindingdirection=TwoWay" FontMargin="0.003" XName="PasswordButton" Margin="0,0,0.025,0.02" />
            <TemplateAsset Template="StandardButtonTemplate" Position="0, 0, 0.75, 0.08">
              <FontAsset Text="X" FontColor="Color.White" Position="0, 0, 0.08, 0.08" FontFamily="SegoeLight" FontMargin="0.02" />
              <FontAsset Text="Test Button" FontColor="Color.White" Position="0.08, 0, 0.67, 0.08" FontFamily="SegoeLight" FontMargin="0.02" />
            </TemplateAsset>
          </StackAsset>
        </RectangleAsset>
      </BoxShadowAsset>
    </Screen>

### Using Breeze.Storage

There are three storage systems available (all using same interface)

* FileSystemStorage - for dealing with raw file system
* UserStorage - for dealing with UserStorage (my documents etc - depends on platform)
* DatfileStorage - for dealing with a pak file (one per project currently)

There are then a number of helper classes, until documentation improves, I suggest intellisense, but an example:

    if (Solids.Breeze.Storage.UserStorage.FileExists("Logins.json"))
    {
    	logInData = Solids.Breeze.Storage.UserStorage.ReadJson<List<LogInData>>("Logins.json");
    }

Also included is PakBuilder project- used to construct pak files for Breeze.Storage.DatfileStorage - this can be used to construct a pak file as part of the build process (which is how I currently use it) meaning you are just deploying two files (a .pak and a .toc) and you are not forced to use Content Pipeline.


### Using Breeze.Benchmark

    using (new Breeze.BenchMark("Benchmark name"))
    {
    	//Long Running Task
    }
    
### Using Breeze.Debug

	BreezeDebug.WriteLine("oh hai");
