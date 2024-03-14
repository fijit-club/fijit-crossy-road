const plugin = {
  setScore: function (score) {
    try {
      const data = { event: 'SET_SCORE', payload: { score } };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to dispatch event');
    }
  },
  vibrate: function (value) {
    try {
	  value = UTF8ToString(value);
      const data = { event: 'VIBRATE', payload: { value } };
      if(window.ReactNativeWebView) window.ReactNativeWebView.postMessage(JSON.stringify(data));
	    if(window.dispatchReactUnityEvent) window.dispatchReactUnityEvent('gameEvent', JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to dispatch event ',e);
    }
  },
  restart: function () {
    try {
      const data = { event: 'RESTART' };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to dispatch event');
    }
  },
  buyAsset: function (assetId) {
    try {
      assetId = UTF8ToString(assetId);
      const data = { event: 'BUY_ASSET', payload: { assetId } };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to post message');
    }
  },
  updateCoins: function (coinsChange) {
    try {
      const data = { event: 'UPDATE_COINS', payload: coinsChange };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to post message');
    }
  },
  updateExp: function (expChange) {
    try {
      const data = { event: 'UPDATE_EXP', payload: { expChange } };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to post message');
    }
  },
  load: function () {
    try {
      const data = { event: 'LOAD' };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to post message');
    }
  },
  registerVisibilityChangeEvent: function () {
    document.addEventListener("visibilitychange", function () {
      SendMessage("Bridge", "OnVisibilityChange", document.visibilityState);
    });
    if (document.visibilityState != "visible")
      SendMessage("Bridge", "OnVisibilityChange", document.visibilityState);
	  
  },setSavedata: function (savedata) {
    try {
      savedata = UTF8ToString(savedata);
      const data = { event: 'SET_SAVEDATA', payload: { savedata } };
      if(window.ReactNativeWebView) window.ReactNativeWebView.postMessage(JSON.stringify(data));
	  if(window.dispatchReactUnityEvent) window.dispatchReactUnityEvent('gameEvent', JSON.stringify(data));
	 
    } catch (e) {
      console.warn('Failed to post message');
    }
	},
  upgradeAsset: function (assetId,attributeId,level) {
    try {
	assetId = UTF8ToString(assetId);
	attributeId = UTF8ToString(attributeId);
      const data = { event: 'UPGRADE_ASSET', payload: { "assetId":assetId ,"attributeId":attributeId ,"level":level } };
      window.ReactNativeWebView.postMessage(JSON.stringify(data));
    } catch (e) {
      console.warn('Failed to dispatch event');
    }
  },
};
	
mergeInto(LibraryManager.library, plugin);
