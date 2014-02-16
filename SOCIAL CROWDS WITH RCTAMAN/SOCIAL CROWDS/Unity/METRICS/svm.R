effortData <- read.csv('effortSVM.txt',head=TRUE)
dataAll <- subset(effortData,select=c(-Class))
classesAll <- subset(effortData,select=Class)

model <- svm(dataAll,classesAll,type='C',kernel='radial', cross = 9)
summary(model)

dataRightHand <- subset(effortData,select=c(spaceR, weightR, timeR, flowR))
model2 <- svm(dataRightHand ,classesAll,type='C',kernel='radial', cross = 9)
summary(model2)


postureData <- read.csv('postureSVM.txt',head=TRUE)
dataPostureAll <- subset(postureData ,select=c(-Class))
classesPostureAll <- subset(postureData ,select=Class)
model3 <- svm(dataPostureAll ,classesPostureAll ,type='C',kernel='radial', cross = 9)
summary(model3)


effortHistograms<- read.csv('effortHistograms.txt',head=TRUE)
dataHistogram <- subset(effortHistograms,select=c(-Class))
classesHistogram <- subset(effortHistograms,select=Class)
model4 <- svm(dataHistogram ,classesHistogram ,type='C',kernel='radial', cross = 9)
summary(model4)


dataHistogramRightHand <- subset(effortHistograms,select=c(spaceR, weightR, timeR, flowR))
model5 <- svm(dataHistogramRightHand ,classesHistogram ,type='C',kernel='radial', cross = 9)
summary(model5)


dataHistogramSmall <- subset(effortHistograms,select=c(spaceR, weightR, spaceL, weightL))
model6 <- svm(dataHistogramSmall ,classesHistogram ,type='C',kernel='radial', cross = 9)
summary(model6)

dataHistogramSmall <- subset(effortHistograms,select=c(spaceR, weightR))
model6 <- svm(dataHistogramSmall ,classesHistogram ,type='C',kernel='radial', cross = 9)
summary(model6)

postureHistograms<- read.csv('postureHistograms.txt',head=TRUE)
dataHistogramPosture <- subset(postureHistograms,select=c(-Class))
classesHistogramPosture <- subset(postureHistograms,select=Class)
model7 <- svm(dataHistogramPosture ,classesHistogramPosture ,type='C',kernel='radial', cross = 9)
summary(model7)


postureHistograms<- read.csv('postureHistograms.txt',head=TRUE)
dataHistogramPostureSmall <- subset(postureHistograms,select=c(abdomenTwist, chestBend, headBend))
classesHistogramPosture <- subset(postureHistograms,select=Class)
model8 <- svm(dataHistogramPostureSmall ,classesHistogramPosture ,type='C',kernel='radial', cross = 9)
summary(model8)


postureHistograms<- read.csv('postureHistograms.txt',head=TRUE)
dataHistogramPostureSmall <- subset(postureHistograms,select=c(chestBend))
classesHistogramPosture <- subset(postureHistograms,select=Class)
model9 <- svm(dataHistogramPostureSmall ,classesHistogramPosture ,type='C',kernel='radial', cross = 9)
summary(model9)

