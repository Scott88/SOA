#import "AppDelegateListener.h"
#import <ItavioSdk/ItavioSdk.h>

@interface ItavioAppDelegateListener : NSObject<AppDelegateListener>

+(ItavioAppDelegateListener *)sharedInstance;

@end


static ItavioAppDelegateListener *_instance = [ItavioAppDelegateListener sharedInstance];

@implementation ItavioAppDelegateListener

+(ItavioAppDelegateListener *)sharedInstance {
    return _instance;
}

+ (void)initialize {
    if(!_instance) {
        _instance = [[ItavioAppDelegateListener alloc] init];
    }
}

- (id)init {
    if(_instance != nil) {
        return _instance;
    }
    
    self = [super init];
    if(!self)
        return nil;
    
    _instance = self;
    
    UnityRegisterAppDelegateListener(self);
    
    return self;
}

#pragma mark - Unity AppDelegateListener Callback Methods

- (void)onOpenURL:(NSNotification*)notification {
    NSURL *url = [notification.userInfo objectForKey:@"url"];
    [[ITSdk sharedInstance] handleLink:url];
}

@end