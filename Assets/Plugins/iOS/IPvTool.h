//
//  IPvTool.h
//  Unity-iPhone
//
//  Created by Mac pro on 2018/4/3.
//

#import <Foundation/Foundation.h>

@interface IPvTool : NSObject
+(NSString *)getIPv6 : (const char *)mHost :(const char *)mPort;
@end
