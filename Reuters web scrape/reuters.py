from urllib.request import urlopen as uReq
from bs4 import BeautifulSoup as soup 

def scrape_reuters(file_name="reuters_articles.csv"):
	my_url= "https://uk.reuters.com"
	uClient = uReq(my_url)
	page_html = uClient.read()
	uClient.close()

	page_soup = soup(page_html, "html.parser")
	articles = page_soup.findAll("article")
	remove = "\n"
	remove2= "\t"
	count = 0
	#File writting
	f = open(file_name, 'w')
	headers = "Title,Timestamp,URL\n"
	f.write(headers)

	for article in articles:
		count +=1
		if article.findAll("span",{"class": "timestamp"}):
			timestamp = article.findAll("span",{"class": "timestamp"})[0].text
		title = article.text.replace(timestamp, "").replace(remove2,"").replace(remove,"").replace(",","|")
		link =  article.a["href"]
		full_url = my_url + link.replace("/uk.reuters.com/","")
		f.write(title + "," + timestamp + "," + full_url + "\n")
	f.close()

if __name__ == '__main__':
        scrape_reuters()
		