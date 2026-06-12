mergeInto(LibraryManager.library, {
  TrackGA4Event: function(eventNamePtr, jsonParamsPtr) {
    var eventName = UTF8ToString(eventNamePtr);
    var jsonParams = UTF8ToString(jsonParamsPtr);
	console.log('[GA4] Tracked: ' + eventName);  // ← agregá esto
    try {
      var params = JSON.parse(jsonParams);
      if (typeof gtag !== 'undefined') {
        gtag('event', eventName, params);
      }
    } catch(e) {
      console.warn('[GA4Bridge] Error parsing params:', e);
    }
  }
});