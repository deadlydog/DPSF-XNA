<?php include "_HTMLPreHead.php"; // Includes the HTML version to use ?>
<title>DPSF (Dynamic Particle System Framework) for XNA</title>
<meta name="description" content="DPSF (Dynamic Particle System Framework) - Create custom particle systems in XNA quickly and easily." />
<meta name="keywords" content="DPSF, Dynamic, Particle, Particle System, Particle Systems, Framework, XNA, C#, Custom, Flexible, API, Library, Tool, Free, Engine, Particle Engine, Particle Engines, Effect, Particle Effect, Particle Effects, Code" />
<?php include "_HTMLHead.php"; // Includes the Head information ?>
</head>

<body>
<?php $SelectedTab = "WhatIsDPSF"; ?>
<?php include '_CommonSiteElements.php'; ?>

<div id="Content">
<h1>What is DPSF</h1>

<p>DPSF (Dynamic Particle System Framework) is a tried and tested, free <strong>programmer's</strong> tool for creating custom particle systems in <strong>XNA</strong> quickly and easily.</p>

<p>Unlike other particle system APIs / libraries, DPSF is flexible and allows you to code your own custom behaviors into the particle system; <strong>You are not limited
to only using the parameters provided by the framework</strong>. You can create and control your own particle properties to make just about any effect you can imagine.</p>

<p>Incorporate particle effects into your project within a matter of minutes by using the provided Default Classes.</p>

<p>Upload particle systems you create to <a href="http://forums.xnaparticles.com">the DPSF forums</a> and download particle systems created by others.</p>

<p>Check out the <a href="DemoVideos.php">demo videos</a> to see some of the things you can do with DPSF, or go ahead and <a href="Download.php">download DPSF</a> and 
try it out for yourself.</p>

<p>If you use DPSF in your project, be sure to post a link to your project on the <a href="http://forums.xnaparticles.com">the DPSF forums</a>.

<h2>Features</h2>
<p>Here is a list of some of the features DPSF provides:</p>
<ul>
    <li>A single API for multiple platforms: supports 2D and 3D particles for Windows, Xbox 360, Windows Phone, and Zune.  Also supports Android  via <a href="http://monogame.codeplex.com" target="_blank">MonoGame</a> (currently waiting on a <a href="https://github.com/mono/MonoGame/issues/1039" target="_blank">MonoGame fix for WinRT support</a> (i.e. Windows Store)).</li>
    <li>Easily integrates with graphics engines, including <a href="http://www.synapsegaming.com" target="_blank">Synapse Gaming's SunBurn engine</a>.</li>
    <li>Full API documentation is provided in the help file, as well as in the <a href="DPSFHelp/index.html">online help documentation</a>.</li>
    <li>Tutorials and their source code are provided in the installer. The tutorials (without source code) are also available in the <a href="DPSFHelp/index.html">online help documentation</a>.</li>
    <li>Allows particle systems to be created in minutes by using the Default Particle Systems provided; Just set values for the built-in parameters, such as position, veleocity, acceleration, rotation, external force, start/end color, etc.</li>
    <li>The Default Particle Systems may be extended, allowing you to provide any extra required functionality. Want your particles to have a weight property that determines how fast they accelerate? Or do you want your particles to follow a specific path or pattern? You can code the behavior to make it happen!</li>
    <li>Templates to start from are provided to make creating new particle systems quick and easy.</li>
    <li>You write the particle system code, giving you full control over the particle system and its particles, allowing you to create any type of particle system effects you desire. Your imagination is the limit.</li>
    <li>Easy to integrate into existing projects; just add a reference to a dll file.</li>
    <li>Use the built-in Effects (i.e. shaders) as well as custom Effects.</li>
    <li>Modify the default Effect (i.e. shaders) to create new custom Effects quickly and easily.</li>
    <li>Particle System Managers are provided to make updating and drawing many particle systems easy.</li>
    <li>An Animations class is provided for easily creating animated particles.</li>
    <li>Easily create a sequence of images, tile-sets, or animated gifs displaying particle system animations.</li>
    <li>Particle systems may be implemented as DrawableGameComponents if required.</li>
</ul>

</div>

<?php include '_Footer.php'; ?>
</body>
</html>
