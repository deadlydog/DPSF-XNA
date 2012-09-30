        <div id="GoogleAdLarge">
            <script type="text/javascript"><!--
            google_ad_client = "pub-0037930831928769";
            /* DPSF 728x90 */
            google_ad_slot = "1340834769";
            google_ad_width = 728;
            google_ad_height = 90;
            //-->
            </script>
            <script type="text/javascript"
            src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
            </script>
        </div>
        
        <div id="Footer">
            This page was last updated on <?php print(date("M j, Y", getlastmod())); ?>
        </div>
    </div> <!-- Close the MenuAndContent div from _CommonSiteElements.php -->
</div> <!-- Close the ContentWrapper div from _CommonSiteElements.php -->

<?php //include '_SideColumn.php'; ?>

<!-- Add the User Quote li items to the DOM, now that the DOM is fully created -->
<script type="text/javascript">  
	gDPSF.InitializeUserQuotes(); 
</script>

<!-- Starts the User Quotes carousel -->
<script type="text/javascript">  
	$(function() {  
	$(".jcarousel-container").jCarouselLite({  
			vertical: true,  
			visible: 1,  
			auto:2250,  
			speed:2000  
		});  
	});  
</script>

<!-- Google Analytics Code -->
<script type="text/javascript">
  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-2386298-5']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();
</script>