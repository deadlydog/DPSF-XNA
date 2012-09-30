<!-- Link this Google Adsense account to the Analyatics account -->
<script type="text/javascript">
window.google_analytics_uacct = "UA-2386298-5";
</script>

<div id="ContentWrapper">
    <div id="Title">
        <img class="Logo" src="/Images/DPSFLogo.png" alt="DPSF (Dynamic Particle System Framework)" title="Dynamic Particle System Framework" />
        
        <!-- News Ticker Demo taken from: http://www.webdesignbooth.com/create-a-vertical-scrolling-news-ticker-with-jquery-and-jcarousel-lite/ -->
        <!-- Script is started from _Footer.php -->
        <div id="UserQuotes" class="jcarousel-container">
            <ul id="UserQuotesList" />
        </div>
        
        <span class="Icons">
            <a href="RSS.xml"><img class="Stacked" src="/Images/RSS-icon.png" alt="RSS" title="RSS Feed for DPSF" /></a>
            <a href="http://www.facebook.com/pages/DPSF/165492630135404?v=page_getting_started#!/pages/DPSF/165492630135404" target="_blank"><img class="Stacked" src="Images/Facebook-icon.png" alt="DPSF on Facebook" title="DPSF Facebook Page" /></a>
            <a href="http://twitter.com/DPSFXNA" target="_blank"><img class="Stacked" src="Images/Twitter-icon.png" alt="DPSF on Twitter" title="DPSF on Twitter" /></a>
        </span>
    </div>

    <div class="Clear"></div>
    
    <div id="MenuAndContent">
        <div id="Menu">
            <ul>
                <li><a href="index.php" <?php if ($SelectedTab == "WhatIsDPSF"){print("class='SelectedTab'");}?>>What is DPSF</a></li>
                <li><a href="Download.php" <?php if ($SelectedTab == "Download"){print("class='SelectedTab'");}?>>Download</a></li>
                <li><a href="ProjectsThatUseDPSF.php" <?php if ($SelectedTab == "ProjectsThatUseDPSF"){print("class='SelectedTab'");}?>>Projects that use DPSF</a></li>
                <li><a href="DemoVideos.php" <?php if ($SelectedTab == "Videos"){print("class='SelectedTab'");}?>>Demo Videos</a></li>
                <li><a href="FAQ.php" <?php if ($SelectedTab == "FAQ"){print("class='SelectedTab'");}?>>FAQ</a></li>
                <li><a href="Support.php" <?php if ($SelectedTab == "Support"){print("class='SelectedTab'");}?>>Support</a></li>
                <li><a href="Contact.php" <?php if ($SelectedTab == "Contact"){print("class='SelectedTab'");}?>>Contact</a></li>
            </ul>
        </div>
        <div class="Clear"></div>
