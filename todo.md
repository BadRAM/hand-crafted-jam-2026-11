Handmade jam todo:
 - Kira-PH Box2d
 - Delete everything
 - Upstream interpolator
 - Better scene system

Migration notes:
 - Audio pan has been rescaled from 0..1 to -1..+1, default pan needs to be updated.
 - Complain to Kira that ContactEvents is a property rather than a method and that this could mislead a user into calling the underlying function more than necessary. (Maybe not a problem, seems like the underlying function is pretty light.)