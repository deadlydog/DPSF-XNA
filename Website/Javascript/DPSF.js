// Global variables
var gImgElementID = null;	// The ID of the img element to update
var gGIFSrc = null;			// The GIF that the img element should display
var gDPSF = new DPSF();

// Swaps the img from displaying a JPG to displaying a GIF
function SwapJPGToGIF(_ElementID)
{
	// Get a handle to the Img to update
	var hImg = document.getElementById(_ElementID);

	// If the JPG is being shown
	if (hImg.src.search(/.jpg/i) != -1)
	{
		// Save the Src to the GIF
		gGIFSrc = hImg.src.replace(/.jpg/i, ".gif");
		
		// Save the Element's ID
		gImgElementID = _ElementID;
		
		// Swap the image to show the loading image
		hImg.src = "Images/Transparent.png";

		// Call the function to show the GIF instead of the loading image.
		// NOTE: We use setTimeout so that the display is updated to show the loading image
		// before switching to showing the GIF, so the loading image shows while the GIF loads.
		setTimeout("_SwapJPGToGIF()", 0);
	}
	// Else the GIF is being displayed already
	else
	{
		// Cause the animation to restart from the beginning
		hImg.src = hImg.src;
	}
}

function _SwapJPGToGIF()
{
	// Update the img to show the GIF
	document.getElementById(gImgElementID).src = gGIFSrc;
}

// Toggles the given element's Visible property.
// Returns false if the element does not exist.
function ToggleVisibility(_ElementID, _bMakeVisible)
{
	// Get a handle to the Element
	var hElement = document.getElementById(_ElementID);
	
	// If we did not get a handle to the Element
	if (hElement == null)
	{
		// Return that the specified ElementID doesn't exist
		return false;	
	}
	
	// If the Visibility property to use is specified
	if (_bMakeVisible != null)
	{
		// If the Element should be visible
		if (_bMakeVisible)
		{
			// Make it visible
			hElement.style.display = "block";
		}
		// Else it should not be visible
		else
		{
			// So make it not visible
			hElement.style.display = "none";
		}
	}
	// Else the Visibility property to use was not specified, so toggle the current Visibility
	else
	{
		// If the Element is not visibile
		if (hElement.style.display == "none")
		{
			// Make it visible
			hElement.style.display = "block";
		}
		// Else the Element is visible
		else
		{
			// So make it not visible
			hElement.style.display = "none";
		}
	}
	
	// Return that the update was successful
	return true;
}

function ToggleVisibilityOfAllFAQAnswers()
{
	// Get if the first Answer is visible or not
	var hElement = document.getElementById("Answer0");
	var bMakeAllAnswersVisible = hElement.style.display == "none" ? true : false;	
	
	// Loop through and toggle each answer until we reach one that doesn't exist
	var iCount = 0;	
	while (ToggleVisibility("Answer" + iCount, bMakeAllAnswersVisible))
	{
		// Increment the counter to update the next Answer
		iCount++;			
	}
	
	iCount = 0;	
	while (ToggleVisibility("CodeAnswer" + iCount, bMakeAllAnswersVisible))
	{
		// Increment the counter to update the next Answer
		iCount++;			
	}
}

function DPSF()
{
	////////////////////////////////////////////////////////////////////////////
    // Public Functions:
    this.InitializeUserQuotes = InitializeUserQuotes;
    
    ////////////////////////////////////////////////////////////////////////////
    // Private Variables:
    var mRoot = this;                   // This variable is used to access the "this.variables" above from within the functions
	
	// This function runs when the DOM is constructed
	function InitializeUserQuotes()
	{
		// Define all of the quotes to display
		var Quotes = new Array();
		var Authors = new Array();
		Quotes[0] = '"I just wanted to write to thank you for making my life that much easier."';
		Authors[0] = '- Domenic Datti';
		Quotes[1] = '"As a programmer who was looking for a particle system, that was not too complex to use with XNa and C#, i have found this to be very useful."';
		Authors[1] = '- Steve Andrews';
		Quotes[2] = '"Thanks for this great and awesome particle library"';
		Authors[2] = '- J2T';
		Quotes[3] = '"I\'ve really appreciated your help, patience with me, quick replies and even dll changes."';
		Authors[3] = '- amadis77';
		Quotes[4] = '"Excellent support! Many thanks. What a great asset for my XNA projects!"';
		Authors[4] = '- ggblake';
		Quotes[5] = '"The XNA (Creator\'s Club) example just is not flexible enough for me, but your API is outstanding!"';
		Authors[5] = '- Domenic Datti';
		Quotes[6] = '"Hey I have been using DPSF for a little while for a little game I\'m working on, It is a great particle engine"';
		Authors[6] = '- Daniel Armstrong, Lead Programmer at 2.0 Studios';
		Quotes[7] = '"Well thank you for a very nice piece of software,...you could very easily sell this product."';
		Authors[7] = '- Steve Andrews';
		Quotes[8] = '"Thanks once again for the great support."';
		Authors[8] = '- ggblake';
		Quotes[9] = '"Hello daniel, many many thanks for a wonderful piece of programming."';
		Authors[9] = '- Steve';
		Quotes[10] = '"DPSF is so flexible, that I\'m sure it will be able to fit exactly what I am trying to do."';
		Authors[10] = '- Domenic Datti';
		Quotes[11] = '"It really is lots of fun trying out DPFS, and i think its the best software around for particles."';
		Authors[11] = '- Steve';
		Quotes[12] = '"DPSF - I Like!"';
		Authors[12] = '- charcon';
		Quotes[13] = '"It is very nice to see that its not all about money, and that you are prepared to let others use (DPSF) for no cost at all."';
		Authors[13] = '- Steve Andrews';
		Quotes[14] = '"Thank you for your help and an excellent framework!"';
		Authors[14] = '- amadis77';
		Quotes[15] = '"Thank you very much :-) You\'re a star!"';
		Authors[15] = '- HaikuJock';
		Quotes[16] = '"I mentioned to Synapsegaming that DPSF integrates well with Sunburn\'s lighting system...just a plug for a great product"';
		Authors[16] = '- ggblake';
		Quotes[17] = '"Thanks a bunch for your help. your system has saved me a ton of time for not only this, but for the several effects i\'ve setup."';
		Authors[17] = '- Firebase';
		Quotes[18] = '"Many thanks for the quick reply! :)"';
		Authors[18] = '- persepee';
		Quotes[19] = '"Anddd everything is fixed. That was some pretty slick detective work. Thanks again! = D"';
		Authors[19] = '- Feep';
		Quotes[20] = '"We\'ve used DPSF for almost all weapon effects in the game(fireballs, wildfire, etc.) and also for a fire and smoke. DPSF is awesome!"';
		Authors[20] = '- Vantosik';
		Quotes[21] = '"I love the particle system you developed."'
		Authors[21] = '- Jason Koohi';
		Quotes[22] = '"The integration with the Sunburn Engine was simple and straightforward and the support Daniel gave us was fantastic!"'
		Authors[22] = '- FrontlineFire'
		Quotes[23] = '"DPSF was an awesome find and it fit the bill perfectly. The samples were great, the API was simple and the helps docs were fantastic."'
		Authors[23] = '- holophone3d'
		Quotes[24] = '"We found DPSF very easy to use and will no doubt be using it in future projects."'
		Authors[24] = '- HaikuJock'
		Quotes[25] = '"Overall, I couldn\'t of asked for a better particle system to integrate into my app."'
		Authors[25] = '- holophone3d'
		Quotes[26] = '"I\'ve been working with your particle system for a few hours now and am loving it. Thank you for dedicating your time to make such a great library =)"'
		Authors[26] = '- Mike Ylosvai'
		Quotes[27] = '"Love this easy to use particle system."'
		Authors[27] = '- CuriousPandaGames'
		Quotes[28] = '"You and your software have enriched the web and science. I thank you very much."'
		Authors[28] = '- Gene'
		Quotes[29] = '"Firstly thanks for DPSF. In no time whatsoever I have lovely particle effects all over my game."'
		Authors[29] = '- pingcrosby'
		Quotes[30] = '"You\'re my hero bro, I can\'t thank you enough :). A free particle system and a helpful coder, you really couldn\'t ask for more. Thanks for everything!"'
		Authors[30] = '- chazzwazzle'
		Quotes[31] = '"Your particle system is amazing, thank you for developing it! :)"'
		Authors[31] = '- OneBeeKay'
		Quotes[32] = '"Thanks for this wonderful project"'
		Authors[32] = '- Fayez R. El-Far, Eng. M.Sc. CSI, Chief Engineer / Director'
		Quotes[33] = '"integrated @DPSFXNA in my windows phone game in an afternoon. Piece of cake! #xna #gamedev"'
		Authors[33] = '- @CaranElmoth, via Twitter'
		Quotes[34] = '"It\'s really a great and easy to use particle system."'
		Authors[34] = '- TesseractGames'

		
		// Get a handle to the UserQuotesList
		var hList = document.getElementById('UserQuotesList');
		
		// Get a random quote to start at
		var startIndex = Math.floor(Math.random() * Quotes.length);
		
		// Add all of the Quotes to the User Quotes List
		var count = 0;
		while (count < Quotes.length)
		{
			// Calculate which index to access, making sure to not go out of bounds
			var i = startIndex + count;
			if (i >= Quotes.length) 
			{ 
				i -= Quotes.length; 
			}
			
			// Add this Quote to the List
			hList.innerHTML += ' <li><div class="Quote">' + Quotes[i] + '</div><div class="QuoteAuthor">' + Authors[i] + '</div><div class="Clear"></div></li>';
			count++;
		}
	}
}