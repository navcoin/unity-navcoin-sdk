// Creating functions for the Unity
mergeInto(LibraryManager.library, {

   Connect: function ()
   {
      try
      {
         chrome.runtime.sendMessage("mehadhhadnkkdajgmdoebkgfldobcded",{method:"connect"}, response =>
         {
            if (!response)
            {
               console.log('No extension');
               return;
            }
            else
            {
               console.log(response);
               unityInstance.SendMessage('Wallet', 'Process', JSON.stringify(response));
            }
        });
     }
      catch(e)
      {
         console.log(e);
         unityInstance.SendMessage('Wallet', 'Process', JSON.stringify({ message: "No extension found",extension_not_found:true}));
      }
   },
   GetNFTCollections: function ()
   {
      try
      {
         chrome.runtime.sendMessage("mehadhhadnkkdajgmdoebkgfldobcded",{method:"list_nft_collections"}, response =>
         {
            if (!response)
            {
               console.log('No extension');
               return;
            }
            else
            {
               console.log(response);
               unityInstance.SendMessage('Wallet', 'ProcessNFTCollections', JSON.stringify(response));
            }
        });
     }
      catch(e)
      {
         console.log(e);
         unityInstance.SendMessage('Wallet', 'ProcessNFTCollections', JSON.stringify({ message: "No extension found",extension_not_found:true}));
      }
   },   
   // Function with the text param
   PassTextParam: function (text) {
      // Convert bytes to the text
      var convertedText = Pointer_stringify(text);

      // Show a message as an alert
      window.alert("You've passed the text: " + convertedText);
   },

   // Function with the number param
   PassNumberParam: function (number) {
      // Show a message as an alert
      window.alert("The number is: " + number);
   },

   // Function returning text value
   GetTextValue: function () {
      // Define text value
      var textToPass = "You got this text from the plugin";

      // Create a buffer to convert text to bytes
      var bufferSize = lengthBytesUTF8(textToPass) + 1;
      var buffer = _malloc(bufferSize);

      // Convert text
      stringToUTF8(textToPass, buffer, bufferSize);

      // Return text value
      return buffer;
   },

   // Function returning number value
   GetNumberValue: function () {
      // Return number value
      return 2020;
   }
});