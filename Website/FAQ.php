<?php include "_HTMLPreHead.php"; // Includes the HTML version to use ?>
<title>DPSF FAQ (Dynamic Particle System Framework)</title>
<meta name="description" content="DPSF (Dynamic Particle System Framework) FAQ. Frequently asked questions about using DPSF to create particle systems in XNA." />
<meta name="keywords" content="DPSF, Dynamic, Particle, Particle System, Particle Systems, Framework, XNA, C#, Custom, Flexible, API, Library, Tool, Free, Flexible, Engine, Particle Engine, Particle Engines, Effect, Particle Effect, Particle Effects, Code, FAQ, Question, Answer" />
<?php include "_HTMLHead.php"; // Includes the Head information ?>
</head>

<body>
<?php $SelectedTab = "FAQ"; ?>
<?php include '_CommonSiteElements.php'; ?>

<div id="Content">
<h1>DPSF Frequently Asked Questions</h1>

<span class="Clickable" onClick="ToggleVisibilityOfAllFAQAnswers()">Expand / Collapse all answers</span>

<h3>General FAQ</h3>
<ul class="FAQ">
	<li><span class="Clickable" onClick="ToggleVisibility('Answer0')">Is DPSF free? (License info)</span></li>
    <p class="Answer" id="Answer0" style="display:none">
    	DPSF is <strong>100% free</strong> to use in any application, although <a href="Download.php">donations</a> are appreciated :)  If you are releasing an 
        application that uses DPSF, you <strong>must reference DPSF somewhere in your application.</strong> It is recommended that you display the DPSF splash screen 
        (players can skip splash screen by pressing a button), but not required; you can just show the DPSF Logo (image) or mention DPSF in text somewhere in the application
        instead. If the only place that DPSF is mentioned in your application is in the credits, then they should be accessible from the main menu.
        The DPSF Logo and Splash Screen Particle System can be found in the DPSF install directory in the Logos directory, typically "C:\DPSF\Logos".<br /><br />
        Putting our logo somewhere on your website (if you have one) with it linking back to this website would also be appreciated,
        but is not required. We also recommend announcing any software releases that uses DPSF on the <a href="http://forums.xnaparticles.com">DPSF Forums</a>, 
        as this will help drive more traffic to your website. Also, if you post any videos of your software, remember to include "DPSF" in the video's tags/keywords so
        that people searching for "DPSF" will find your videos.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer1')">What is a particle system?</span></li>
    <p class="Answer" id="Answer1" style="display:none">
    	A particle system is a piece of software used to control the movement, behavior, and visualization of many particles, where a particle is 
        typically represented on screen as a texture (i.e. image).  When the particles are percieved as a whole they appear to be a single object.  Particle systems
        are typically used to create special effects, such as fire, smoke, explosions, dust, clouds, rain, snow, water, sparks, fur, hair, magic spells, etc.</p>

	<li><span class="Clickable" onClick="ToggleVisibility('Answer2')">What is DPSF?</span></li>
    <p class="Answer" id="Answer2" style="display:none">
    	DPSF (Dynamic Particle System Framework) is a framework for creating custom particle systems in Microsoft's XNA framework.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer3')">Is it easy to create simple particle systems using DPSF?</span></li>
    <p class="Answer" id="Answer3" style="display:none">
    	Yes, DPSF provides Default Particle System Classes with built in support for things like velocity, acceleration, friction, rotation, external forces, etc.
        so creating a simple particle system is as easy as specifying a few parameters.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer4')">Is DPSF only good for creating simple particle systems?</span></li>
    <p class="Answer" id="Answer4" style="display:none">
	    No, DPSF is great for creating both simple and complex particle systems. DPSF was originally designed to allow you to create complex or unusual 
        particle systems, such as those often needed in research applications. DPSF allows you to specify your own particle properties and behaviors. So 
        if you wanted to, for example, give your particles a Temperature property and have them rise when they are hot and fall when they are cool, this 
        can easily be accomplished with a few lines of code.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer5')">Are there any tutorials on how to use DPSF?</span></li>
    <p class="Answer" id="Answer5" style="display:none">
    	Yes! DPSF provides tutorials and their source code in the DPSF installer. The tutorials can be found in the <strong>DPSF Help.chm</strong> included 
        with the installer, which is also <a href="DPSFHelp/index.html">available online here</a>. You can also view the particle system source code of the 
        DPSF Demo to see how all of the effects in the Demo are created and learn from them as well.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer6')">Is there a GUI editor that can be used to create particle systems?</span></li>
    <p class="Answer" id="Answer6" style="display:none">
    	Not at the moment, but one is currently in development. At the moment though, all particle system parameters must be specified by code.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer7')">Is it easy to integrate DPSF into my existing project?</span></li>
    <p class="Answer" id="Answer7" style="display:none">
    	Yes, it simply involves adding a reference to a dll file. From there you can create a particle system class (or simply grab one from the demo samples
        provided) and start using it in your application. You can have a particle system in your application within a matter of minutes.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer8')">Can DPSF only be used with XNA applications that use the Game class?</span></li>
    <p class="Answer" id="Answer8" style="display:none">
    	No, the Game class is not required. DPSF can be used with games as well as WinForms applications; as long as they use an XNA GraphicsDevice to do
        the rendering, as shown in the <a href="http://creators.xna.com/en-us/sample/winforms_series1">WinForms samples on the XNA Creators Club website</a>.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('Answer9')">Does DPSF run on the CPU or GPU?</span></li>
    <p class="Answer" id="Answer9" style="display:none">
    	In order for DPSF to maintain its flexibility and to support the complex behaviors that it does, while being easy to use, it must run on the CPU. 
        However, since you can still write the GPU shaders that DPSF uses, you can create a GPU particle system using DPSF, but you would have to write all 
        of the shader code yourself (nothing is provided by DPSF for you to do this yet). In the future DPSF may get official support for GPU particle systems, 
        but at the moment DPSF is designed for CPU particle systems.</p>
        
    <li><span class="Clickable" onClick="ToggleVisibility('Answer10')">Can I use your particle systems and images in my project?</span></li>
    <p class="Answer" id="Answer10" style="display:none">
    	Yes, you can use any of the particle systems provided by DPSF in your projects for free, and feel free to modify them as you need. You can also
        use any of the particle image textures provided by DPSF for free in your projects as well. If you release your project to the general public though
        it must have a DPSF splash screen (users can skip splash screen).</p>
        
    <li><span class="Clickable" onClick="ToggleVisibility('Answer11')">Does DPSF support XNA 4.0?</span></li>
    <p class="Answer" id="Answer11" style="display:none">
        Yes, DPSF fully supports both XNA 3.1 and 4.0, including full support for Windows PCs, Xbox 360, Windows Phone 7, and the Zune.</p>
</ul>

<h3>Code FAQ</h3>
<ul class="FAQ">
	<li><span class="Clickable" onClick="ToggleVisibility('CodeAnswer0')">How do I kill a particle before it reaches the end of its lifetime?</span></li>
    <p class="Answer" id="CodeAnswer0" style="display:none">
    	Set the particle's <strong>NormalizedElapsedTime</strong> property to 1.0.</p>
        
    <li><span class="Clickable" onClick="ToggleVisibility('CodeAnswer1')">Why aren't my particles showing up?</span></li>
    <p class="Answer" id="CodeAnswer1" style="display:none">
    	Make sure you are calling the particle system's <strong>Update()</strong> and <strong>Draw()</strong> functions, as well as its 
        <strong>SetWorldViewProjectionMatrices()</strong> function directly before the Draw() function. If you have particle systems contained within other 
        particle systems, make sure these functions are being called on the inner particle systems as well. Other things to check include making sure the particle 
        system's <strong>Enabled</strong> and <strong>Visible</strong> properties are both true, and that the <strong>SimulationSpeed</strong> property is greater than zero.</p>
        
    <li><span class="Clickable" onClick="ToggleVisibility('CodeAnswer2')">My Quad particles show up, but don't always face the camera (i.e. are not billboards).</span></li>
    <p class="Answer" id="CodeAnswer2" style="display:none">
    	All of the Quad particle systems must be told where the Camera is each update call in order to orient themselves to face it. If your Quad particles are visible, but 
        do not always face the camera (and you want them to), you are likely forgetting to update the Quad particle system's <strong>CameraPosition</strong> property to 
        the position of the camera before calling the particle system's Update() function. The other thing to make sure of is that you are also using an Everytime Event to
        update each particle to face the camera each frame (i.e. this.ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera).<br /><br />
        An alternative to having to do this is to use a Sprite3DBillboard particle system instead of a Quad particle system, as these are designed to always
        face the camera; even without setting up an Everytime Event to rotate the particle towards the camera.</p>
    
    <li><span class="Clickable" onClick="ToggleVisibility('CodeAnswer3')">After drawing one of the particle systems, the textures on my models no longer show up.</span></li>
    <p class="Answer" id="CodeAnswer3" style="display:none">
    	When a DPSF particle system is drawn, it typically changes the Graphics Device's Samper State's U, V, and W Address modes to Border. To restore the textures
        to your models, you will need to change these modes back to Wrap before drawing your models, using the code:<br /><br />
        GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;<br />
        GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;<br />
        GraphicsDevice.SamplerStates[0].AddressW = TextureAddressMode.Wrap;</p>
        
	<li><span class="Clickable" onClick="ToggleVisibility('CodeAnswer4')">My particle systems are broken after updating DPSF versions.</span></li>
    <p class="Answer" id="CodeAnswer4" style="display:none">
    	Check out the <a href="DPSFHelp/index.html">DPSF help</a> documentation. There is an entire section describing the common code breaking changes that will need to 
        be fixed from one version of DPSF to the next.</p>
</ul>

<p>Have a question that is not answered here? Post it on <a href="http://forums.xnaparticles.com">the DPSF Forums</a>.</p>

</div>

<?php include '_Footer.php'; ?>
</body>
</html>
