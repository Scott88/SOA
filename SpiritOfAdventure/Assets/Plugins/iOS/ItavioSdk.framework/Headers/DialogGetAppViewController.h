//
//  pop.h
//  PopupTest
//
//  Created by Joselyn O'Connor on 2015-03-26.
//  Copyright (c) 2015 Joselyn O'Connor. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>

#define APP_STORE_URL @"itms-apps://appstore.com/kinderguardianinc/itavio"

@interface DialogGetAppViewController : UIViewController

@property (weak, nonatomic) IBOutlet UIView *popView;
@property (weak, nonatomic) IBOutlet UILabel *message;

- (void)showInView:(UIView *)aView animated:(BOOL)animated;

@end