jsb.reflection = new JavaScriptObjCBridge();
var ret = jsb.reflection.callStaticMethod("NativeOcClass",
    "callNativeWithReturnString:andContent:",
    "done",
    "done");