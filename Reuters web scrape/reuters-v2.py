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
	headers = "Title,Timestamp,URL, Article\n"
	f.write(headers)

	for article in articles:
		count +=1
		if article.findAll("span",{"class": "timestamp"}):
			timestamp = article.findAll("span",{"class": "timestamp"})[0].text
		title = article.text.replace(timestamp, "").replace(remove2,"").replace(remove,"").replace(",","|")
		link =  article.a["href"]
		full_url = my_url + link.replace("/uk.reuters.com/","")
		uClient = uReq(full_url)
		article_html = uClient.read()
		uClient.close()
		article_soup = soup(article_html, "html.parser")
		if article_soup.findAll("div",{"class": "StandardArticleBody_body_1gnLA"}):
			article_unclean = article_soup.findAll("div",{"class": "StandardArticleBody_body_1gnLA"})[0]
			if article_unclean:
				if article_soup.findAll("div",{"class": "Attribution_container_28wm1"}):
					author = article_soup.findAll("div",{"class": "Attribution_container_28wm1"})[0].text
					article = article_unclean.text.replace(author, "").replace(article_unclean.span.text,"").replace(article_unclean.a.text, "")
				else:
					article = article_unclean.text.replace(article_unclean.span.text,"").replace(article_unclean.a.text, "")
				f.write(str((title + "," + timestamp + "," + full_url + "," + article.replace(",","|")+ "\n").encode("utf-8")).replace("\xe2\x80\x99", ""))
				f.write("\n")
			else:
				f.write(str((title + "," + timestamp + "," + full_url + "," + " "+ "\n").encode("utf-8")).replace("\xe2\x80\x99", ""))
				f.write("\n")
	f.close()

if __name__ == '__main__':
        scrape_reuters()
		