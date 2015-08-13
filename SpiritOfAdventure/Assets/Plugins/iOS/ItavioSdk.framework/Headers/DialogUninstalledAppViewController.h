//
//  pop.h
//  PopupTest
//
//  Created by Joselyn O'Connor on 2015-03-26.
//  Copyright (c) 2015 Joselyn O'Connor. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>

@interface DialogUninstalledAppViewController : UIViewController

@property (weak, nonatomic) IBOutlet UIView *popView;
@property (weak, nonatomic) IBOutlet UILabel *email;
@property (weak, nonatomic) IBOutlet UITextField *password;

- (void)showInView:(UIView *)aView withEmail:(NSString*)email animated:(BOOL)animated;

@end