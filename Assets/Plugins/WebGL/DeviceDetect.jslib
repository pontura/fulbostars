mergeInto(LibraryManager.library, {
  IsMobileDevice: function () {
    var ua = navigator.userAgent || navigator.vendor || window.opera;
    var isMobileUA = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(ua);
    var isTouchDevice = navigator.maxTouchPoints > 1;
    var isCoarsePointer = window.matchMedia("(pointer: coarse)").matches;
    return (isMobileUA || isTouchDevice || isCoarsePointer) ? 1 : 0;
  }
});