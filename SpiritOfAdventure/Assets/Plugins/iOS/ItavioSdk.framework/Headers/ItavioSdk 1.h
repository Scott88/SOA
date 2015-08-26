//
//  ItavioSdk.h
//  ItavioSdk
//
//  Created by Matthew Pichette on 2015-03-21.
//  Copyright (c) 2015 Itavio. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for ItavioSdk.
FOUNDATION_EXPORT double ItavioSdkVersionNumber;

//! Project version string for ItavioSdk.
FOUNDATION_EXPORT const unsigned char ItavioSdkVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <ItavioSdk/PublicHeader.h>

#import "ItavioApi.h"

#define ITAVIO_SECRET_KEY_ID @""
#define ITAVIO_SECRET_KEY @""
#define CREDENTIALS [[ITCredentials alloc] init: ITAVIO_SECRET_KEY_ID securedBy: ITAVIO_SECRET_KEY]